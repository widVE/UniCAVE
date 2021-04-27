# Python Cluster Launcher
 Simple cluster launcher for use with UniCAVE

## Requirements
[PyYAML](https://pypi.org/project/PyYAML/)

## Usage
Create a `config.yaml` file in the same directory as the cluster launcher script. There is an example file in the repository for reference. Then simply run the `ClusterLauncher.py` script. Unity instances created by the script will be closed when the script or the head node exits.
Currently, the script is designed to launch multiple instances on one machine. It does not yet support launching instances over a network.