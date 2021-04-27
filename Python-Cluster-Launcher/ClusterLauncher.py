import os
import sys
import time
import subprocess
import yaml
import pathlib

class ClusterLauncher:
    def __init__(self, config_yaml):
        self.Config = config_yaml

    def Launch(self):
        #read config
        with open(self.Config, 'r') as yf:
            config = yaml.safe_load(yf)

            #launch head node
            head = ClusterLauncher.LaunchUniCAVEWindow(config["build-path"], config["head-node"])

            #wait a bit before launching child nodes
            if config["head-wait"] is not None:
                time.sleep(config["head-wait"])

            #launch child nodes
            children = []
            for child_node in config["child-nodes"]:
                children.append(ClusterLauncher.LaunchUniCAVEWindow(config["build-path"], child_node))
                #wait a bit between launching each child
                if config["child-wait"] is not None:
                    time.sleep(config["child-wait"])

            #poll head node process
            done = False
            while not done:
                if head.poll() is not None:
                    done = True
                time.sleep(config["sleep-time"])

            #when done, close child processes and exit
            for child in children:
                child.kill()

    @staticmethod
    def LaunchUniCAVEWindow(path, machine_name=None):
        args = [path, "-popupWindow"]
        if machine_name is not None:
            args = args + ["overrideMachineName", machine_name]
        return subprocess.Popen(args)

if __name__ == "__main__":
    ClusterLauncher(os.path.join(pathlib.Path(__file__).parent.absolute(), "config/input_test.yaml")).Launch()