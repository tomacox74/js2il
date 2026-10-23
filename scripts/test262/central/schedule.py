"""One existing weekly timer, native recovery; manual MVP remains the default."""
import argparse
import json
from pathlib import Path


def collection_kind(event, requested=''):
    if event == 'schedule':
        return 'native'
    if event != 'workflow_dispatch' or requested not in ('', 'native', 'mvp-composite'):
        raise ValueError('Unsupported collection trigger or kind')
    return requested or 'mvp-composite'


def main():
    p=argparse.ArgumentParser()
    p.add_argument('--event',required=True)
    p.add_argument('--kind',default='')
    p.add_argument('--output',required=True)
    p.add_argument('--github-output',required=True)
    args=p.parse_args()
    kind=collection_kind(args.event,args.kind)
    result={'event':args.event,'kind':kind,'automatic_publication':False,
            'budget_policy':'new-actions-run-allocation; retries-share-existing-charges'}
    Path(args.output).parent.mkdir(parents=True,exist_ok=True)
    Path(args.output).write_text(json.dumps(result,sort_keys=True)+'\n')
    with Path(args.github_output).open('a') as stream:
        stream.write('kind='+kind+'\n')


if __name__=='__main__':
    main()
