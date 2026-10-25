"""Disposable synthetic scale probe. No PostgreSQL connection or source data.

Run from the repository root with PYTHONPATH=. and an output directory.
"""
import sys,json,time,uuid,hashlib
from pathlib import Path
sys.path.insert(0,'tests/test262/central')
from test_reconciliation import row,write_export,HEADER
from scripts.test262.central import reconciliation as current
import argparse
parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('--fixtures',type=int,default=1000)
parser.add_argument('--output',required=True)
args=parser.parse_args()
if not 1<=args.fixtures<=100000:raise ValueError('fixtures must be between 1 and 100000')
root=Path(args.output);root.mkdir(parents=True,exist_ok=True)
size=args.fixtures
fixtures=[str(uuid.uuid5(uuid.NAMESPACE_URL,'scale-fixture-'+str(n))) for n in range(size)]
provenances=[str(uuid.uuid5(uuid.NAMESPACE_URL,'scale-provenance-'+str(n))) for n in range(120)]
snapshots=[str(uuid.uuid5(uuid.NAMESPACE_URL,'scale-snapshot-'+str(n))) for n in range(80)]
def rows():
 yield row('corpora',corpus_id='c',inventory_state='sealed',expected_fixture_count=size,revision='a'*40)
 yield row('repository_corpora',corpus_id='c')
 for n,f in enumerate(fixtures):
  yield row('fixtures',fixture_id=f,corpus_id='c',upstream_path=f'test/{n}.js',content_sha256='hash',metadata_state='valid',is_support_file=False,dependency_manifest_digest=None,metadata={'uninterpreted':'x'*200})
  yield row('fixture_variants',fixture_id=f,variant='strict',required=True)
 for p in provenances:
  yield row('provenances',provenance_id=p,corpus_id='c',evidence_kind='native')
  for f in fixtures:yield row('provenance_fixture_eligibility',provenance_id=p,fixture_id=f,eligibility='runnable',diagnostic={'description':'x'*120},reason_codes=[])
 for s in snapshots:
  yield row('registration_snapshots',snapshot_id=s,source_revision='a'*40,state='sealed')
  for n,f in enumerate(fixtures):yield row('registrations',snapshot_id=s,upstream_path=f'test/{n}.js',source_file=f'generated/{n}.cs',registration_kind='native',fixture_id=f,native_fixture_sha256='a'*64)
 yield row('imports',import_id='i',source_schema='native-v4',expected_counts={'attempts':0},imported_counts={'observations':0},limitations={'artifact_history_complete':False})
 for n in range(size*50):yield row('legacy_control_records',import_id='i',source_table='historical_audit',source_key=hashlib.sha256(str(n).encode()).hexdigest(),document={'retained_uninterpreted_control':'x'*400})
path=root/'synthetic.ndjson';start=time.monotonic();receipt=write_export(path,rows());print(json.dumps({'stage':'generated','rows':sum(receipt['counts'].values()),'bytes':path.stat().st_size,'seconds':time.monotonic()-start}),flush=True)
events=[]
started=time.monotonic()
def progress(**event):
 event['elapsed_seconds']=round(time.monotonic()-started,3)
 if not events or event.get('stage')!=events[-1].get('stage') or event.get('table')!=events[-1].get('table'):
  print(json.dumps(event),flush=True)
 events.append(event)
report=current.reconcile(path,receipt,seconds=1800,progress=progress)
elapsed=time.monotonic()-started
result={'scope':'Synthetic frozen input; no production runtime guarantee','rows':sum(receipt['counts'].values()),
 'bytes':path.stat().st_size,'seconds':elapsed,'verified':report['verified'],'logical_sha256':report['logical_sha256'],
 'stages':events,'table_counts':receipt['counts']}
(root/'scale-report.json').write_text(json.dumps(result,indent=2)+'\n')
path.unlink()
print(json.dumps({k:v for k,v in result.items() if k not in ('stages','table_counts')}),flush=True)
if not report['verified']:raise SystemExit(1)
