"""Real disposable Git recovery; GitHub and coordinator transports are simulated."""
import hashlib
import json
from pathlib import Path
import subprocess
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import patch

from scripts.test262.central import publication as pub, commands


class InterruptedPublication(RuntimeError):
    pass


class Coordinator:
    def __init__(self, base):
        self.base=base; self.publication=None; self.transitions=[]

    def one(self, table, **key):
        if table=='native_batches':
            return {'state':'sealed','validation_id':'validation'}
        if table=='validation_events':
            return {'conclusion':'success','target_revision':self.base}
        if table=='publications':
            return dict(self.publication) if self.publication else None
        raise AssertionError(table)

    def read(self, table, filters):
        return [dict(self.publication)] if self.publication and self.publication['state']==filters['state'] else []

    def put(self, table, rows):
        if table=='publications':
            if not self.publication:
                self.publication=dict(rows[0],state='reserved',version=0,pr_number=None,expected_head=None)
            else:
                for key,value in rows[0].items():
                    if self.publication[key]!=value:
                        raise ValueError('Immutable reservation conflict')
        else:
            raise AssertionError(table)

    def call(self, op, table, key, version, update):
        if version!=self.publication['version']:
            raise ValueError('Stale reservation version')
        self.transitions.append(dict(update))
        self.publication.update(update,version=version+1)
        return dict(self.publication)


class PublicationRecoveryTests(unittest.TestCase):
    def setUp(self):
        self.directory=tempfile.TemporaryDirectory()
        self.addCleanup(self.directory.cleanup)
        self.root=Path(self.directory.name); self.remote=self.root/'remote.git'; self.work=self.root/'work'
        self.run_git('init','--bare',str(self.remote),cwd=self.root)
        self.run_git('clone',str(self.remote),str(self.work),cwd=self.root)
        self.git('checkout','-b','master')
        (self.work/'fixture.txt').write_text('before\n')
        self.commit('baseline'); self.base=self.git('rev-parse','HEAD')
        self.git('push','origin','master')
        (self.work/'fixture.txt').write_text('after\n')
        self.patch=subprocess.check_output(['git','diff','--binary',self.base,'--'],cwd=self.work)
        self.git('checkout','--','fixture.txt')
        files={'batch.json':json.dumps({'batch':'batch','context':{'revision':self.base}}),
               'patch.diff':self.patch.decode(),
               'manifest.json':json.dumps({'valid':True,'base_revision':self.base,'patch_digest':hashlib.sha256(self.patch).hexdigest()}),
               'body.md':'Disposable transport test'}
        for name,content in files.items():(self.root/name).write_text(content)
        self.args=SimpleNamespace(repository='repo-id',repository_name='test/repo',
              batch_file=str(self.root/'batch.json'),patch=str(self.root/'patch.diff'),
              manifest=str(self.root/'manifest.json'),body=str(self.root/'body.md'))
        self.client=Coordinator(self.base); self.prs=[]; self.creates=0; self.fail_after_create=False
        self.original_output=subprocess.check_output
        for target,value in ((pub,'REPO'),(commands,'REPO')):
            patcher=patch.object(target,value,self.work);patcher.start();self.addCleanup(patcher.stop)
        patcher=patch.object(pub,'gh',self.github);patcher.start();self.addCleanup(patcher.stop)
        patcher=patch.object(pub.subprocess,'check_output',self.output);patcher.start();self.addCleanup(patcher.stop)

    def run_git(self,*args,cwd=None):
        return subprocess.run(['git',*args],cwd=cwd or self.work,check=True,capture_output=True,text=True).stdout.strip()

    def git(self,*args):
        return self.run_git(*args)

    def commit(self,message):
        self.git('add','fixture.txt')
        self.git('-c','user.name=Pilot','-c','user.email=pilot@example.invalid','-c','commit.gpgsign=false','commit','-m',message)

    def github(self,*args):
        if args[:2]==('pr','list'):
            return [dict(pr) for pr in self.prs]
        raise AssertionError(args)

    def output(self,command,**kwargs):
        if command[0]!='gh':
            return self.original_output(command,**kwargs)
        self.assertEqual(command[1:3],['pr','create'])
        self.creates+=1
        head=self.git('rev-parse','HEAD')
        self.prs.append({'number':81,'state':'OPEN','headRefOid':head,
                         'baseRefName':'master','headRepository':{'nameWithOwner':'test/repo'}})
        self.git('push','origin',head+':refs/pull/81/head')
        if self.fail_after_create:
            raise InterruptedPublication('PR created but acknowledgement lost')
        return 'https://github.com/test/repo/pull/81\n'

    def reset_checkout(self):
        self.git('checkout','--detach',self.base)

    def seed_pushed_reservation(self,parent=None):
        row={'publication_id':pub.identity('repo-id','publication','batch'),'repository_id':'repo-id','batch_id':'batch',
             'branch_name':'test262/native-central-batch',
             'patch_sha256':'\\x'+hashlib.sha256(self.patch).hexdigest(),'manifest_sha256':'\\x'+hashlib.sha256((self.root/'manifest.json').read_bytes()).hexdigest()}
        self.client.put('publications',[row])
        if parent:
            self.git('checkout','--detach',parent)
        (self.work/'fixture.txt').write_text('after\n')
        self.commit('test262: native central batch batch')
        head=self.git('rev-parse','HEAD')
        self.git('push','origin',head+':refs/heads/'+row['branch_name'])
        return head

    def test_loss_after_pr_create_resumes_without_duplicate(self):
        self.fail_after_create=True
        with self.assertRaises(InterruptedPublication):pub.publish(self.client,self.args)
        self.assertEqual(self.client.publication['state'],'reserved')
        head=self.prs[0]['headRefOid']; self.reset_checkout(); self.fail_after_create=False
        result=pub.publish(self.client,self.args)
        self.assertEqual(result['head'],head)
        self.assertEqual(result['pr_number'],81)
        self.assertEqual(self.creates,1)
        self.assertTrue(result['requires_independent_pr_ci'])

    def test_crash_after_push_keeps_branch_and_reservation_for_original_retry(self):
        head=self.seed_pushed_reservation()
        result=pub.recover_reservation(self.client,'test/repo',self.client.publication)
        self.assertEqual(result['recovery'],'resume-original-batch')
        self.assertEqual(self.client.publication['state'],'reserved')
        self.assertEqual(self.git('ls-remote','--heads','origin',self.client.publication['branch_name']).split()[0],head)
        self.assertEqual(self.client.transitions,[])
        self.reset_checkout()
        result=pub.publish(self.client,self.args)
        self.assertEqual(result['head'],head)
        self.assertEqual(self.creates,1)

    def test_no_visible_branch_or_pr_does_not_cancel_possible_live_publisher(self):
        self.seed_pushed_reservation()
        self.git('push','origin','--delete',self.client.publication['branch_name'])
        result=pub.recover_reservation(self.client,'test/repo',self.client.publication)
        self.assertEqual(result['state'],'reserved')
        self.assertEqual(self.client.transitions,[])

    def test_merged_and_closed_prs_recovered_after_branch_deletion(self):
        head=self.seed_pushed_reservation(); branch=self.client.publication['branch_name']
        self.git('push','origin',head+':refs/pull/81/head');self.git('push','origin','--delete',branch)
        for state,expected in [('MERGED','merged'),('CLOSED','closed-deferred')]:
            with self.subTest(state=state):
                self.client.publication.update(state='reserved',version=0,expected_head=None,pr_number=None)
                self.prs=[{'number':81,'state':state,'headRefOid':head,'baseRefName':'master','headRepository':{'nameWithOwner':'test/repo'}}]
                result=pub.recover_reservation(self.client,'test/repo',dict(self.client.publication))
                self.assertEqual(result['state'],expected)
                self.assertEqual(result['expected_head'],head)
                self.assertFalse(self.git('ls-remote','--heads','origin',branch))

    def test_closed_pr_retry_never_recreates_deleted_branch(self):
        self.fail_after_create=True
        with self.assertRaises(InterruptedPublication):pub.publish(self.client,self.args)
        self.prs[0]['state']='CLOSED'; branch=self.client.publication['branch_name']
        self.git('push','origin','--delete',branch); self.reset_checkout()
        with self.assertRaisesRegex(ValueError,'closed'):pub.publish(self.client,self.args)
        self.assertFalse(self.git('ls-remote','--heads','origin',branch))
        self.assertEqual(self.creates,1)
        self.assertEqual(self.client.publication['state'],'closed-deferred')

    def test_same_marker_patch_wrong_parent_is_not_owned(self):
        (self.work/'unrelated.txt').write_text('different parent\n');self.git('add','unrelated.txt')
        self.git('-c','user.name=Pilot','-c','user.email=pilot@example.invalid','commit','-m','different target')
        wrong=self.git('rev-parse','HEAD');head=self.seed_pushed_reservation(parent=wrong)
        self.assertFalse(pub.owned_head(self.client,self.client.publication,head))
        result=pub.recover_reservation(self.client,'test/repo',dict(self.client.publication))
        self.assertEqual(result['state'],'blocked')
        self.assertEqual(self.git('ls-remote','--heads','origin',result['branch_name']).split()[0],head)

    def test_foreign_pr_repository_or_base_not_adopted(self):
        head=self.seed_pushed_reservation();self.git('push','origin',head+':refs/pull/81/head')
        for repository,base in [('other/repo','master'),('test/repo','feature')]:
            with self.subTest(repository=repository,base=base):
                self.client.publication.update(state='reserved',version=0)
                self.prs=[{'number':81,'state':'OPEN','headRefOid':head,'baseRefName':base,'headRepository':{'nameWithOwner':repository}}]
                result=pub.recover_reservation(self.client,'test/repo',dict(self.client.publication))
                self.assertEqual(result['state'],'blocked')

    def test_validation_manifest_must_bind_exact_target_and_patch(self):
        for changes in ({'valid':False},{'base_revision':'b'*40},{'patch_digest':'c'*64}):
            manifest={'valid':True,'base_revision':self.base,'patch_digest':hashlib.sha256(self.patch).hexdigest()}
            (self.root/'manifest.json').write_text(json.dumps(dict(manifest,**changes)))
            with self.assertRaisesRegex(ValueError,'proof'):pub.publish(self.client,self.args)
            self.assertIsNone(self.client.publication)
            self.assertFalse(self.git('ls-remote','--heads','origin','test262/native-central-batch'))

    def test_recorded_pr_with_foreign_scope_or_changed_head_is_not_marked_merged(self):
        head=self.seed_pushed_reservation()
        branch=self.client.publication['branch_name']
        for changed in ('repository','head'):
            with self.subTest(changed=changed):
                self.client.publication.update(state='open',pr_number=81,expected_head=head,version=0)
                pr={'merged':True,'state':'closed','head':{'sha':head,'ref':branch,'repo':{'full_name':'test/repo'}},'base':{'ref':'master'}}
                if changed=='repository':pr['head']['repo']['full_name']='other/repo'
                else:pr['head']['sha']='f'*40
                with patch.object(pub,'gh',return_value=pr):pub.reconcile_publications(self.client,'test/repo')
                self.assertEqual(self.client.publication['state'],'ownership-lost')

    def test_closed_recovery_precedes_freshness_even_after_master_advances(self):
        self.fail_after_create=True
        with self.assertRaises(InterruptedPublication):pub.publish(self.client,self.args)
        self.prs[0]['state']='MERGED';branch=self.client.publication['branch_name']
        self.git('push','origin','--delete',branch)
        self.reset_checkout();self.git('checkout','-B','master',self.base)
        (self.work/'fixture.txt').write_text('new master target\n');self.commit('master advanced')
        self.git('push','origin','master')
        with self.assertRaisesRegex(ValueError,'closed'):pub.publish(self.client,self.args)
        self.assertEqual(self.client.publication['state'],'merged')
        self.assertFalse(self.git('ls-remote','--heads','origin',branch))
        self.assertEqual(self.creates,1)
