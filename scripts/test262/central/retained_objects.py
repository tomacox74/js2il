"""Exact-version encrypted Object Lock receipts shared by backup and restore jobs."""
from datetime import datetime, timedelta, timezone
import hashlib
import json
from pathlib import Path
import re
from urllib.parse import urlsplit

BACKUP_SUFFIX = re.compile(r'\d{4}/\d{2}/\d{2}/\d+-[1-9]\d*')


def destination(uri):
    value=urlsplit(uri)
    if value.scheme!='s3' or not value.netloc or value.query or value.fragment:
        raise ValueError('Invalid S3 destination')
    return value.netloc,value.path.strip('/')


def verify_bucket(s3,bucket):
    versioning=s3.get_bucket_versioning(Bucket=bucket)
    lock=s3.get_object_lock_configuration(Bucket=bucket)['ObjectLockConfiguration']
    default=lock.get('Rule',{}).get('DefaultRetention',{})
    if not (versioning.get('Status')=='Enabled' and lock.get('ObjectLockEnabled')=='Enabled'
            and default.get('Mode')=='COMPLIANCE'
            and (default.get('Days',0)>=35 or default.get('Years',0)>=1)):
        raise ValueError('Versioning and at least 35 days default COMPLIANCE retention required')


def verify_object(s3,bucket,key,kms_key,*,pinned=None,output=None):
    request={'Bucket':bucket,'Key':key}
    if pinned is not None:
        request['VersionId']=pinned['version_id']
    head=s3.head_object(**request)
    version=head.get('VersionId')
    if not version or version=='null' or (pinned is not None and version!=pinned['version_id']):
        raise ValueError('Stored object lacks the required immutable version')
    retention=s3.get_object_retention(Bucket=bucket,Key=key,VersionId=version).get('Retention',{})
    until=retention.get('RetainUntilDate')
    if not (head.get('ServerSideEncryption')=='aws:kms' and head.get('SSEKMSKeyId')==kms_key
            and retention.get('Mode')=='COMPLIANCE' and until is not None
            and until>datetime.now(timezone.utc)):
        raise ValueError('Exact-version encryption/retention failed')
    remote=s3.get_object(Bucket=bucket,Key=key,VersionId=version)
    if remote.get('VersionId')!=version:
        raise ValueError('Downloaded object version mismatch')
    digest=hashlib.sha256();length=0
    from contextlib import nullcontext
    with (Path(output).open('wb') if output else nullcontext()) as dest, remote['Body'] as body:
        for chunk in iter(lambda:body.read(1024*1024),b''):
            digest.update(chunk);length+=len(chunk)
            if dest is not None: dest.write(chunk)
    if length!=head['ContentLength']:
        raise ValueError('Downloaded object length mismatch')
    result={'version_id':version,'content_length':length,'sha256':digest.hexdigest(),
            'encryption':'aws:kms','kms_key':kms_key,'lock_mode':'COMPLIANCE',
            'retain_until':until.isoformat(),'retention_verified':True}
    if pinned is not None and (result['sha256']!=pinned['sha256']
                              or length!=pinned['content_length']):
        raise ValueError('Exact-version bytes differ from receipt')
    return result


def latest_receipt(s3,bucket,base,kms_key,output,*,max_age_hours=48):
    """Select the newest fresh completed receipt; never fall back past a broken newest backup."""
    prefix=base.rstrip('/')+'/' if base else ''
    candidates=[]
    for page in s3.get_paginator('list_objects_v2').paginate(Bucket=bucket,Prefix=prefix):
        for item in page.get('Contents',[]):
            relative=item['Key'][len(prefix):]
            if relative.endswith('/backup-report.json') and BACKUP_SUFFIX.fullmatch(relative[:-len('/backup-report.json')]):
                candidates.append(item)
    if not candidates:
        raise ValueError('No verified backup receipt found')
    selected=max(candidates,key=lambda item:(item['LastModified'],item['Key']))
    now=datetime.now(timezone.utc)
    if not now-timedelta(hours=max_age_hours)<=selected['LastModified']<=now:
        raise ValueError('Latest backup receipt is stale or future dated')
    path=Path(output)/'backup-report.json'
    proof=verify_object(s3,bucket,selected['Key'],kms_key,output=path)
    receipt=json.loads(path.read_text())
    selected_prefix=selected['Key'].removesuffix('/backup-report.json')
    checked=datetime.fromisoformat(receipt.get('checked_at',''))
    if (receipt.get('schema_version')!=1 or receipt.get('backup_verified') is not True
        or receipt.get('bucket')!=bucket or receipt.get('prefix')!=selected_prefix
        or set(receipt.get('objects',{}))!={'catalogue.dump','manifest.json'}
        or checked.tzinfo is None or not now-timedelta(hours=max_age_hours)<=checked<=now):
        raise ValueError('Latest backup receipt is incomplete or misattributed')
    return selected_prefix,receipt,dict(proof,key=selected['Key'])
