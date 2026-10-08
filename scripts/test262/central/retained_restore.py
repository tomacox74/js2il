"""Verify retained S3 backup bytes and restore exclusively into disposable loopback PG17."""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import sys
from urllib.parse import urlsplit


def verify(args):
    import boto3
    import psycopg
    from psycopg.conninfo import conninfo_to_dict

    output = Path(args.output)
    output.mkdir(parents=True, exist_ok=True)
    report = {"restore_verified": False, "independent_retention_verified": False,
              "production_cutover": False, "stage": "validate-input"}
    report_path = output / "restore-report.json"
    try:
        # A production DSN cannot be substituted even by environment configuration.
        dsn = os.environ["TEST262_RESTORE_DATABASE_URL"]
        connection = conninfo_to_dict(dsn)
        if connection.get("host") not in ("127.0.0.1", "localhost"):
            raise ValueError("Restore must use loopback")
        if connection.get("dbname") != "catalogue_retained_restore":
            raise ValueError("Restore must use the dedicated disposable database")
        with psycopg.connect(dsn) as db:
            version = db.execute("SHOW server_version_num").fetchone()[0]
            if not 170000 <= int(version) < 180000:
                raise ValueError("PostgreSQL 17 required")
            if db.execute("SELECT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname IN ('test262','test262_reporting')) OR to_regclass('public.perf_results') IS NOT NULL").fetchone()[0]:
                raise ValueError("Disposable target is not empty")
        if not re.fullmatch(r"\d{4}/\d{2}/\d{2}/\d+-[1-9]\d*", args.backup_suffix):
            raise ValueError("Invalid backup suffix")
        uri = urlsplit(os.environ["TEST262_BACKUP_S3_URI"])
        if uri.scheme != "s3" or not uri.netloc or uri.query or uri.fragment:
            raise ValueError("Invalid S3 destination")
        prefix = "/".join(filter(None, (uri.path.strip("/"), args.backup_suffix)))
        s3 = boto3.client("s3")
        report.update(bucket=uri.netloc, prefix=prefix, objects={})
        report["stage"] = "retention-metadata"
        # Bucket settings alone are insufficient: inspect the actual stored versions.
        versioning = s3.get_bucket_versioning(Bucket=uri.netloc)
        lock = s3.get_object_lock_configuration(Bucket=uri.netloc)["ObjectLockConfiguration"]
        default = lock.get("Rule", {}).get("DefaultRetention", {})
        bucket_ok = (versioning.get("Status") == "Enabled"
                     and lock.get("ObjectLockEnabled") == "Enabled"
                     and default.get("Mode") == "COMPLIANCE"
                     and (default.get("Days", 0) >= 35 or default.get("Years", 0) >= 1))
        report["bucket_retention_verified"] = bucket_ok
        retention_ok = bucket_ok
        now = datetime.now(timezone.utc)
        for name in ("catalogue.dump", "manifest.json"):
            key = prefix + "/" + name
            head = s3.head_object(Bucket=uri.netloc, Key=key)
            version = head.get("VersionId")
            if not version or version == "null":
                raise ValueError("Backup object has no immutable version ID")
            retention = s3.get_object_retention(Bucket=uri.netloc, Key=key, VersionId=version).get("Retention", {})
            until = retention.get("RetainUntilDate")
            object_ok = (head.get("ServerSideEncryption") == "aws:kms"
                         and head.get("SSEKMSKeyId") == os.environ["TEST262_BACKUP_KMS_KEY"]
                         and retention.get("Mode") == "COMPLIANCE"
                         and until is not None and until > now)
            retention_ok = retention_ok and object_ok
            report["objects"][name] = {
                "version_id": version, "content_length": head["ContentLength"],
                "encryption": head.get("ServerSideEncryption"),
                "kms_key": head.get("SSEKMSKeyId"), "lock_mode": retention.get("Mode"),
                "retain_until": until.isoformat() if until else None,
                "retention_verified": object_ok}
            report["stage"] = "download-exact-version"
            response = s3.get_object(Bucket=uri.netloc, Key=key, VersionId=version)
            if response.get("VersionId") != version:
                raise ValueError("Downloaded object version mismatch")
            digest = hashlib.sha256()
            byte_count = 0
            with (output / name).open("wb") as stream:
                with response["Body"] as body:
                    for chunk in iter(lambda: body.read(1024 * 1024), b""):
                        stream.write(chunk)
                        digest.update(chunk)
                        byte_count += len(chunk)
            if byte_count != head["ContentLength"]:
                raise ValueError("Downloaded object length mismatch")
            report["objects"][name]["sha256"] = digest.hexdigest()
        report["stage"] = "digest"
        manifest = json.loads((output / "manifest.json").read_text())
        if report["objects"]["catalogue.dump"]["sha256"] != manifest["sha256"]:
            raise ValueError("Downloaded dump digest differs from manifest")
        report["digest_verified"] = True
        report["independent_retention_verified"] = retention_ok
        report["stage"] = "disposable-restore"
        result = subprocess.run(
            [sys.executable, "-m", "scripts.test262.central.backup", "restore",
             "--archive", str(output / "catalogue.dump"),
             "--manifest", str(output / "manifest.json")],
            check=True, capture_output=True, text=True)
        # Keep dump/SQL and raw server diagnostics out of logs and report artifacts.
        restored = json.loads(result.stdout.strip().splitlines()[-1])
        if not restored.get("restore_verified"):
            raise ValueError("Restore tool did not verify")
        with psycopg.connect(dsn) as db:
            state, epoch = db.execute("SELECT deployment_state,minimum_writer_epoch FROM test262.schema_contract").fetchone()
            if state != "shadow" or epoch != int(manifest["contract"]["minimum_writer_epoch"]) + 1:
                raise ValueError("Restored authority fencing mismatch")
            if db.execute("SELECT to_regclass('public.perf_results')").fetchone()[0] is not None:
                raise ValueError("Performance table unexpectedly restored")
        report.update(restore_verified=True, restored_authority=state, restored_epoch=epoch,
                      writer_bindings_disabled=True, row_counts=manifest["row_counts"],
                      view_counts=manifest["view_counts"], stage="complete")
        if not retention_ok:
            raise ValueError("Restore passed but retention/encryption acceptance failed")
        return 0
    except Exception as error:
        report["error_class"] = type(error).__name__
        response = getattr(error, "response", {})
        if isinstance(response, dict):
            report["aws_error_code"] = response.get("Error", {}).get("Code")
        return 1
    finally:
        report["checked_at"] = datetime.now(timezone.utc).isoformat()
        report_path.write_text(json.dumps(report, indent=2) + "\n")
        print(json.dumps({k: report[k] for k in
                         ("stage", "restore_verified", "independent_retention_verified")}))
        # Only the sanitized report is retained by the workflow.
        for name in ("catalogue.dump", "manifest.json"):
            (output / name).unlink(missing_ok=True)


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--backup-suffix", required=True)
    parser.add_argument("--output", required=True)
    raise SystemExit(verify(parser.parse_args()))
