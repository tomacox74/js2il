"""Read-only check of a dedicated login, including through a session pooler."""
import argparse
import os
import sys


class CheckError(ValueError):
    """A diagnostic composed only of static text and known setting names."""


def failure_message(error):
    if isinstance(error, CheckError):
        return 'Connection check failed: ' + str(error)
    # Driver/parser exceptions can contain credentials; do not display their text.
    return 'Connection check failed (' + type(error).__name__ + '). Check login, TLS and environment settings.'


def check(login, trust):
    import psycopg
    from psycopg.conninfo import conninfo_to_dict
    names = ('TEST262_DATABASE_URL', 'TEST262_REPOSITORY_ID',
             'TEST262_PRODUCER_ID', 'TEST262_AUTHORITY_EPOCH')
    missing = [name for name in names if not os.getenv(name)]
    if missing:
        raise CheckError('Missing environment settings: ' + ', '.join(missing))
    try:
        epoch = int(os.environ['TEST262_AUTHORITY_EPOCH'])
    except ValueError:
        raise CheckError('TEST262_AUTHORITY_EPOCH must be an integer') from None
    dsn = os.environ['TEST262_DATABASE_URL']
    sslmode = conninfo_to_dict(dsn).get('sslmode', 'require')
    if sslmode not in ('require', 'verify-ca', 'verify-full'):
        raise CheckError('TLS required: use sslmode=require, verify-ca or verify-full')
    with psycopg.connect(dsn, sslmode=sslmode, connect_timeout=20, autocommit=True) as db:
        db.execute('BEGIN READ ONLY')
        db.execute("SET LOCAL statement_timeout='15s'")
        if db.execute('SHOW transaction_read_only').fetchone()[0] != 'on':
            raise CheckError('Read-only transaction required')
        actual, contract = db.execute('SELECT session_user, test262.api_contract()').fetchone()
        if actual != login:
            raise CheckError('Wrong database login: check the username in the database URL secret for the selected workflow')
        if not contract:
            raise CheckError('No enabled catalogue binding for this database login')
        expected = {'repository_id': os.environ['TEST262_REPOSITORY_ID'],
                    'producer_id': os.environ['TEST262_PRODUCER_ID'],
                    'minimum_writer_epoch': epoch,
                    'api_contract_version': 1, 'permission': 'coordinator', 'trust_class': trust}
        for key, value in expected.items():
            if contract.get(key) != value:
                raise CheckError('Catalogue binding mismatch: ' + key)
        if trust == 'legacy' and contract['deployment_state'] not in ('schema-only', 'shadow'):
            raise CheckError('Historical import requires inactive authority (schema-only or shadow)')
        db.execute('ROLLBACK')
    print('PASS: TLS connection and ' + login + ' binding verified.')
    print('Authority state:', contract['deployment_state'])
    return contract


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--login', required=True)
    parser.add_argument('--trust', choices=('trusted', 'legacy'), required=True)
    args = parser.parse_args()
    try:
        check(args.login, args.trust)
    except Exception as error:
        # Connection exceptions can contain DSN fragments; never print them.
        print(failure_message(error), file=sys.stderr)
        raise SystemExit(1)
