# CFLogGet

[![Build status](https://ci.appveyor.com/api/projects/status/cmggi5plj1tgni9d?svg=true)](https://ci.appveyor.com/project/nabehiro/cflogget)

Download  CloudFront logs from S3 and store on SQL Server.

# Requirements
- AWS S3 Read Access Key (Secret Key) to download CloudFront logs.
- SQL Server Database to store logs.

# Setup
+ Download CFLogGet package from [releases](https://github.com/nabehiro/CFLogGet/releases)
+ Create a Database on SQL Server to store CloudFront logs.
+ Modify SQL Server Database Connection String in cflogget.exe.config.
+ Run following on the command prompt.  
 `> cflogget.exe register -a <AWS Access Key> -s <AWS Secret Key> -r <AWS region>`  
 This command register access profile with the name *cflogget* on SDK Store of your machine.  
 [AWS region](https://docs.aws.amazon.com/general/latest/gr/rande.html) is *us-east-2*, *ap-south-1* etc.

# Usage (get CloudFront logs)
Run following command.  
```
> cflogget.exe get -b <Bucket Name> -r <Root Directory> -s <Start DateTime> -e <End DateTime>
```
sample
```
> cflogget.exe get -b my-bucket -r my-site -s "2018-01-01 10:00" -e "2018-01-02 12:00"
```

To know more usage detail, run help command
```
> cflogget.exe register -h

> cfloget.exe get -h
```

When `cflogget.exe get` command is completed, a table to store logs is created in the specified database !


