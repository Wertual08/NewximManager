# NewximManager
This software was developed as a utity for the [Newxim](https://github.com/Wertual08/newxim) 
simulator. It provides functionality to run multiple simulation instances in parallel.
It also manages configurations and outputs of the simulator.


## Configuration
Manager configuration can be done via command line arguments or
by configuration file. You can not mix those two and if
```-config``` option was specified the rest ones will be ignored.

### Command line arguments configuration
- ```-t <number of simultaneous simulations>```
- ```-x <path to simulator executable>```
- ```-c <path to simulator configuration file>```
- ```-v <start> <step> <stop>```<br> 
  Specifies starting value, step and final value for the **variable** parameter
- ```-x <exporter>```<br>
  Specifies format of the exported metrics.<br>
  Possible values: ```CSV``` (default)
- ```-o <output file path>```
- ```--<simulator argument name> <simulator argument value>```<br>
  Specifies arguments, forwarded to the simulator. 
  Simulator argument value can contain % simbol which will be
  replaced with **variable**, configured via ```-v```
- ```-config <path to manager config file>```

### File configuration
Any value of ```arguments``` can contain % which will be replaces with **variable** value for each simulator instance.

```yml
common:
  executable_path: <path to simulator executable>
  config_path: <path to simulator configuration file>
  value_start: <startign number of the variable>
  value_step: <step of the variable>
  value_stop: <final value of the variable>
  exporter: <exporter type>
  workers_pool_size: <number of simultaneous simulations>
  arguments:
    <simulator argument name>: <simulator argument value>
    <simulator argument name>: <simulator argument value>
    <simulator argument name>: <simulator argument value>
    ...
```
This segment specifies shared configuration for all of the simulator instances.

```yml
configurations:
- output_path: <path of the output file for this simulations>
  arguments:
    <simulator argument name>: <simulator argument value>
    <simulator argument name>: <simulator argument value>
    <simulator argument name>: <simulator argument value>
    ...
```
```configurations``` section specifies an array of configurations
(each new entry must start with ```-``` ). You can multiple configurations. All of them will be qued one after another.

### Ouptut
As a result of execution, program produces a file for each
configuration. One configutaion corresponds to one 
series of simulations. **Variable** value for each simulation 
is calculated as ```starting value + simulation number * variable step```.