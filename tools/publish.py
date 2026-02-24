#!/bin/python3
from __future__ import annotations

import argparse
import os
import pathlib
import platform
import shutil
import subprocess
import sys
import typing


class Colors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'


def get_version():
    solution_info = origin / 'shared' / 'SolutionInfo.cs'
    version_prefix = '[assembly: AssemblyVersion("'
    version_suffix = '")]'
    # [assembly: AssemblyVersion("1.3.5.20")]
    for line in solution_info.read_text().split('\n'):
        if line.startswith(version_prefix):
            return line.rstrip()[len(version_prefix):-len(version_suffix)]
    raise ValueError('Failed to parse version from ' + str(solution_info))


def print_green(text: str) -> None:
    print(Colors.OKGREEN + text + Colors.ENDC, file=sys.stderr)


def get_lnk_bytes(text: str) -> bytes:
    return b''.join(
        (b'\x00' + chr(code).encode(encoding='ascii'))
        for code in text.encode(encoding='ascii')
    )


def run(
        command: str | os.PathLike,
        args: typing.Sequence[str | os.PathLike],
        timeout: int = 3600,
        check: bool = True,
) -> bool:
    if is_windows:
        if command == 'mono':
            command = args[0]
            args = args[1:]
    args.insert(0, command)
    proc = subprocess.run(args, check=check)
    return proc.returncode == 0

is_windows = platform.system() == 'Windows'
configuration = 'release'
origin = pathlib.Path(__file__).parent.absolute().parent
repos = origin.parent
output = origin / 'out'
version = get_version()

local_dev_dir = pathlib.Path(
    r'D:\distrib\games\mtg' if is_windows else '/home/kolia/mtg/'
)
target_root = local_dev_dir / 'packaged' / version

package_name = 'Mtgdb.Gui.v' + version
package_name_zip = package_name + '.zip'
filelist_txt = 'filelist.txt'
version_txt = 'version.txt'
target = target_root / package_name
target_bin = target / 'bin' / ('v' + version)
util_exe = output / 'bin' / configuration / 'Mtgdb.Util.exe'

yandex_dir = pathlib.Path(
    r'C:\Users\hidal\YandexDisk' if is_windows
    else '/home/kolia/shared'
)
yandex_dir_app = yandex_dir / 'Mtgdb.Gui' / 'app'
remote_dir = yandex_dir_app / 'release'
remote_test_dir = yandex_dir_app / 'test'
remote_deflate_dir = yandex_dir_app / 'deflate'


def build():
    if is_windows:
        run(
            r'C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools'
            r'\MSBuild\Current\Bin\MSBuild.exe', [
                str(origin / 'Mtgdb.sln'),
                '-verbosity:m',
                '-p:Configuration=' + configuration
            ]
        )
    else:
        run('msbuild', [
            str(origin / 'Mtgdb.Mono.sln'),
            '-verbosity:m',
            '-p:Configuration=' + configuration
        ])


def create_publish_dir():
    shutil.rmtree(target_root, ignore_errors=True)
    target.mkdir(parents=True)


def copy_files():

    def ignore_data(path, _):
        if path == str(output / 'data'):
            return (
                'allSets-x.json',
                'AllPrices.json',
                'AllSets.v42.json',
            )
        if path == str(output / 'data' / 'index'):
            return (
                'deck',
                'keywords-test',
                'search-test',
                'suggest-test',
            )
        return ()

    shutil.copytree(
        output / 'data',
        target / 'data',
        ignore=ignore_data
    )
    shutil.copytree(
        output / 'bin' / configuration,
        target_bin,
        ignore=shutil.ignore_patterns('*.xml', '*.pdb')
    )
    shutil.copytree(
        output / 'etc',
        target / 'etc'
    )
    shutil.copytree(
        output / 'images',
        target / 'images',
        ignore=shutil.ignore_patterns('*.jpg', '*.png', '*.txt')
    )

    def ignore_update(path, names):
        ignored = set(
            name for name in names
            if name.endswith('.bak')
            or name.endswith('.zip')
            or name.endswith('.7z')
        )
        if path == str(output / 'update'):
            ignored.update((
                filelist_txt,
                version_txt,
                'app',
                'notifications',
                'megatools-1.9.98-win32',
            ))
        elif path == str(output / 'update' / 'img' / 'art'):
            ignored.update((filelist_txt,))
        return ignored

    shutil.copytree(
        output / 'update',
        target / 'update',
        ignore=ignore_update
    )
    shutil.copytree(
        output / 'color-schemes',
        target / 'color-schemes',
        ignore=shutil.ignore_patterns('current.colors')
    )
    shutil.copytree(
        output / 'charts',
        target / 'charts',
    )
    shutil.copy(
        output.parent / 'LICENSE',
        target / 'LICENSE',
    )
    shutil.copy(
        output / 'start.sh',
        target / 'start.sh',
    )
    (target / 'update' / version_txt).write_text(package_name_zip)


def make_shortcut():
    template_path = origin / 'tools' / 'shortcut.lnk.template'
    lnk_content_template = template_path.read_bytes()
    lnk_content = lnk_content_template.replace(
        get_lnk_bytes('v0.0.0.0'),
        get_lnk_bytes('v' + version)
    )
    (target / 'Mtgdb.Gui.lnk').write_bytes(lnk_content)


def sign_binary_files():
    for match in target.glob('*.vshost.*'):
        match.unlink()
    run('mono', [
        util_exe,
        '-sign',
        target_bin,
        '-output',
        target_bin / filelist_txt
    ])


def create_lzma_compressed_zip():
    run('7z', [
        'a',
        str(target_root / package_name_zip),
        '-tzip',
        '-ir!' + str(target_root / package_name / '*'),
        '-mmt=on',
        '-mm=LZMA',
        '-md=64m',
        '-mfb=64',
        '-mlc=8',
    ])


def sign_zip():
    run('mono', [
        util_exe,
        '-sign',
        target_root / package_name_zip,
        '-output',
        target_root / filelist_txt
    ])


def publish_zip_to_test_update_url():
    for match in remote_test_dir.glob('*.zip'):
        match.unlink()
    shutil.copy(
        target_root / package_name_zip,
        remote_test_dir / package_name_zip
    )
    shutil.copy(
        target_root / filelist_txt,
        remote_test_dir / filelist_txt
    )


def run_installed_app():
    subprocess.Popen(
        ['explorer', pathlib.Path(r'D:\games\mtgdb.gui\Mtgdb.Gui.lnk')]
        if is_windows
        else pathlib.Path('/run/media/kolia/ssd/apps/Mtgdb.Gui/start.sh'),
        close_fds=True
    )


def run_tests():
    run('mono', [
        origin / 'tools' / 'NUnit.Console-3.7.0' / 'nunit3-console.exe',
        origin / 'out' / 'bin' / 'release-test' / 'Mtgdb.Test.dll'
    ])


def prompt_user_confirmation():
    input('Press Ctrl+C to cancel or Enter to continue...')


def publish_update_notification():
    repos_wiki = repos / 'mtgdb.wiki'
    repos_notifications = repos / 'mtgdb.notifications'
    run('git', ['-C', repos_wiki, 'pull'])
    run('mono', [util_exe, '-notify'])
    run('git', ['-C', repos_notifications, 'pull'])
    run('git', ['-C', repos_notifications, 'add', '-A'])
    if run('git', ['-C', repos_notifications, 'commit', '-m', 'auto'], check=False):
        run('git', ['-C', repos_notifications, 'push'])


def publish_zip_to_actual_update_url():
    for match in remote_dir.glob('*.zip'):
        match.unlink()
    shutil.copy(target_root / package_name_zip, remote_dir / package_name_zip)
    shutil.copy(target_root / filelist_txt, remote_dir / filelist_txt)


def create_deflate_compressed_zip():
    (target_root / 'deflate').mkdir()
    run('7z', [
        'a',
        target_root / 'deflate' / 'Mtgdb.Gui.zip',
        '-tzip',
        '-ir!' + str(target_root / package_name / '*'),
        '-x!' + str(pathlib.Path('data') / 'index' / '*'),
        '-x!' + str(pathlib.Path('data') / 'AllPrintings.json'),
        '-x!' + str(pathlib.Path('data') / 'AllPrices.json'),
        '-mm=deflate'
    ])


def upload_deflate_compressed_zip():
    for match in remote_deflate_dir.glob('*.zip'):
        match.unlink()
    for match in (target_root / 'deflate').glob('*'):
        shutil.copy(match, remote_deflate_dir / match.name)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument(
        '-f',
        '--from',
        dest='from_step',
        type=str,
        nargs='?',
        help='skip steps before specified',
    )
    parser.add_argument(
        '-l',
        '--list',
        dest='list_steps',
        action='store_true',
    )
    parser.add_argument(
        '--no-tests',
        dest='no_tests',
        action='store_true',
    )
    parser.add_argument(
        '--no-run',
        dest='no_run',
        action='store_true',
    )
    args = parser.parse_args()

    steps: list[typing.Callable[[], None]] = [
        build,
        create_publish_dir,
        copy_files,
        make_shortcut,
        sign_binary_files,
        create_lzma_compressed_zip,
        sign_zip,
        publish_zip_to_test_update_url,
        *((run_tests,) if not args.no_tests else ()),
        *((run_installed_app,) if not args.no_run else ()),
        prompt_user_confirmation,
        publish_update_notification,
        publish_zip_to_actual_update_url,
        create_deflate_compressed_zip,
        upload_deflate_compressed_zip,
    ]

    skip = bool(args.from_step)
    for step in steps:
        skip &= step.__name__ != args.from_step
        if skip:
            print_green(f'skip step: {step.__name__}')
        else:
            print_green(f'step: {step.__name__}')
            if not args.list_steps:
                step()


if __name__ == "__main__":
    main()
