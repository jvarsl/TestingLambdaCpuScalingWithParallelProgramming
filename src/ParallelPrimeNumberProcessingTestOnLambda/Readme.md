## Rough sketch, description about the tasks

Searches prime numbers in 0 - 10 million range.

Methods:

- DoSearchAsync - splits into 10 tasks and asks to execute in parallel
- DoSearchSync - run all 10 million check in one method (single task)
- DoSearchAsyncTwoTasksComputation - executes DoSearchSync method in parallel

## Learnings

- Did not learn any magical tricks (AWS account was limited to max 3008mb size)
- Sync (single core/CPU utilization) performance plateaued at ~1800mb for ~2500ms time. Found later confirmation https://docs.aws.amazon.com/lambda/latest/dg/configuration-function-common.html#:~:text=At%201%2C769%20MB%2C%20a%20function%20has%20the%20equivalent%20of%20one%20vCPU%20(one%20vCPU%2Dsecond%20of%20credits%20per%20second). At 3600mb two full power cores are reached to run DoSearchSync and DoSearchAsyncTwoTasksComputation(DoSearchSync twice) in ~2500ms.
- In both async methods the gains continue to grow. Unsure how load is being spread, how much cores we have access to at the start but in all parallel tasks more memory led to little better performance so it seems new slow cores were not added and power scaled lineary (no magical second CPU/core appears).

## Collected stats

| Compute    | Size       | DoSearchAsync | DoSearchSync | DoSearchAsyncTwoTasksComputation | Unit |
| ---------- | ---------- | ------------- | ------------ | -------------------------------- | ---- |
| Local      | 10 cores   | 0.22          | 1.2          | 1.2                              | s    |
| AWS Lambda | 512        | 8.7           | 8.6          |                                  | s    |
| AWS Lambda | 1024       | 4.1           | 4            |                                  | s    |
| AWS Lambda | 2000       | 2.1           | 2.5          |                                  | s    |
| AWS Lambda | 3008 (max) | 1.6           | 2.5          |                                  | s    |
| AWS Lambda | 1400       | 3168          | 3207         |                                  | ms   |
| AWS Lambda | 1500       | 2961          |              |                                  | ms   |
| AWS Lambda | 1700       | 2588          |              |                                  | ms   |
| AWS Lambda | 1800       | 2505          |              |                                  | ms   |
| AWS Lambda | 1850       | 2423          |              |                                  | ms   |
| AWS Lambda | 1900       | 2329          | ~2500        |                                  | ms   |
| AWS Lambda | 2000       | 2230          | ~2500        |                                  | ms   |
| AWS Lambda | 2048       | 2190          | 2542         |                                  | ms   |
| AWS Lambda | 2100       | 2129          | ~2500        |                                  | ms   |
| AWS Lambda | 3000       | 1582          | ~2500        |                                  | ms   |
| AWS Lambda | 1024       | 4384          | 4297         | 8668                             | ms   |
| AWS Lambda | 1768       | 2479          | 2508         | 5014                             | ms   |
| AWS Lambda | 1770       | 2544          | 2446         | 4982                             | ms   |
| AWS Lambda | 1850       | 2371          | 2438         | 4752                             | ms   |
| AWS Lambda | 1900       |               |              | 4637                             | ms   |
| AWS Lambda | 2200       |               |              | 3958                             | ms   |
| AWS Lambda | 2700       |               |              | 3228                             | ms   |
| AWS Lambda | 3008       | 1720          | 2462         | 2980                             | ms   |
| AWS Lambda | 3600       | 1247          | 2457         | 2495                             | ms   |
| AWS Lambda | 10240      | 581           | 2437         | 2465                             | ms   |

#### Interesting read but bit conflicting information and also doesn't find magical spot where new core unleashes new powers, it grows lineary

https://stackoverflow.com/questions/66522916/aws-lambda-memory-vs-cpu-configuration

https://web.archive.org/web/20220629183438/https://www.sentiatechblog.com/aws-re-invent-2020-day-3-optimizing-lambda-cost-with-multi-threading?utm_source=reddit&utm_medium=social&utm_campaign=day3_lambda
