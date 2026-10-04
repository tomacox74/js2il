"""Read-only check of a dedicated login, including through a session pooler."""
import argparse
import os
import sys


def check(login, trust):
    import psycopg
    from psycopg.conninfo import conninfo_to_dict
    names = ('TEST262_DATABASE_URL', 'TEST262_REPOSITORY_ID',
             'TEST262_PRODUCER_ID', 'TEST262_AUTHORITY_EPOCH')
    if any(not os.getenv(name) for name in names):
        raise ValueError('Missing catalogue environment setting')
    dsn = os.environ['TEST262_DATABASE_URL']
    sslmode = conninfo_to_dict(dsn).get('sslmode', 'require')
    if sslmode not in ('require', 'verify-ca', 'verify-full'):
        raise ValueError('TLS connection required')
    with psycopg.connect(dsn, sslmode=sslmode, connect_timeout=20, autocommit=True) as db:
        db.execute('BEGIN READ ONLY')
        db.execute("SET LOCAL statement_timeout='15s'")
        if db.execute('SHOW transaction_read_only').fetchone()[0] != 'on':
            raise ValueError('Read-only transaction required')
        actual, contract = db.execute('SELECT session_user, test262.api_contract()').fetchone()
        if actual != login or not contract:
            raise ValueError('Wrong login or missing binding')
        expected = {'repository_id': os.environ['TEST262_REPOSITORY_ID'],
                    'producer_id': os.environ['TEST262_PRODUCER_ID'],
                    'minimum_writer_epoch': int(os.environ['TEST262_AUTHORITY_EPOCH']),
                    'api_contract_version': 1, 'permission': 'coordinator', 'trust_class': trust}
        for key, value in expected.items():
            if contract.get(key) != value:
                raise ValueError('Catalogue binding mismatch: ' + key)
        if trust == 'legacy' and contract['deployment_state'] not in ('schema-only', 'shadow'):
            raise ValueError('Historical import requires inactive authority')
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
        print('Connection check failed (' + type(error).__name__ + '). Check login, TLS and environment settings.', file=sys.stderr)
        raise SystemExit(1)
