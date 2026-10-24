"""Disposable-only fault transport around the supported enlistment entry point."""
import json
import os
from pathlib import Path
import sys
import time
from types import SimpleNamespace
from .client import Client, canonical
from .enlist import run
from .pilot_drills import require


class Transport:
    def __init__(self,client,control):
        self.client,self.control=client,control
        self.claimed=False
        self.interrupted=False

    def __getattr__(self,name):
        return getattr(self.client,name)

    def call(self,operation,*args):
        if operation=='claim' and not self.claimed:
            self.claimed=True
            Path(self.control['ready']).touch()
            until=time.monotonic()+45
            while not Path(self.control['peer_ready']).exists():
                require(time.monotonic()<until,'Independent claim peer timed out')
                time.sleep(.05)
        if operation=='ingest' and not self.interrupted:
            self.interrupted=True
            fault=self.control.get('fault')
            if fault=='pause-before-upload':
                Path(self.control['checkpoint']).write_text(canonical({'observation_id':args[1][0]['observation_id']}))
                # The parent kills only this owned supervisor after durable spool.
                until=time.monotonic()+60
                while time.monotonic()<until:
                    time.sleep(.1)
                raise TimeoutError('Disposable kill checkpoint timed out')
            if fault=='lost-ack':
                self.client.call(operation,*args)
                raise ConnectionError('Disposable committed acknowledgement lost')
            if fault=='offline-before-upload':
                raise ConnectionError('Disposable connection lost before upload')
        return self.client.call(operation,*args)


def main():
    configuration=json.loads(Path(sys.argv[1]).read_text())
    client=Client()
    try:
        info=client.db.info
        require(info.host in ('127.0.0.1','localhost') and info.dbname=='catalogue_pilot' and
                info.user in ('catalogue_pilot_independent_a','catalogue_pilot_independent_b'),
                'Fault transport requires the disposable independent worker login')
        result=run(Transport(client,configuration['control']),SimpleNamespace(**configuration['args']))
        print(canonical(result))
        return 0
    except Exception as error:
        print(canonical({'error_type':type(error).__name__}))
        return 1
    finally:
        client.close()


if __name__=='__main__':
    raise SystemExit(main())
