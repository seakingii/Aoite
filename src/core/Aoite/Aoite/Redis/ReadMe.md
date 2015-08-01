## String 未实现命令

* [BITCOUNT](http://redisdoc.com/string/bitcount.html)
* [BITOP](http://redisdoc.com/string/bitop.html)
* [GETBIT](http://redisdoc.com/string/getbit.html)
* [SETBIT](http://redisdoc.com/string/setbit.html)
* [SETEX](http://redisdoc.com/string/psetex.html) 、[PSETEX](http://redisdoc.com/string/psetex.html)、[SETNX](http://redisdoc.com/string/setnx.html)：因为 [SET](http://redisdoc.com/string/set.html) 命令可以通过参数来实现这三个命令的效果。

## Key 未实现命令

* [MIGRATE](http://redisdoc.com/key/migrate.html)
* [OBJECT](http://redisdoc.com/key/object.html)
* [DUMP](http://redisdoc.com/key/dump.html)
* [RESTORE](http://redisdoc.com/key/restore.html)
* <s>[SORT](http://redisdoc.com/key/sort.html)：为了找到更好的 SORT 设计模式，实现其他所有命令后再去实现它。</s>*2015-01-20*

## Pub/Sub 未实现命令

* 所有命令等找到合适的方式再去实现。

## Connection
* [ECHO](http://redisdoc.com/connection/echo.html)

## Server
* [ClientPause](http://redis.io/commands/client-pause)
* [Monitor](http://redisdoc.com/server/monitor.html)
* [DEBUG OBJECT](http://redisdoc.com/server/debug_object.html)
* [DEBUG SEGFAULT](http://redisdoc.com/server/debug_segfault.html)
* [ROLE](http://redis.io/commands/Role)
* [SHUTDOWN](http://redisdoc.com/server/shutdown.html)
* [PSYNC](http://redisdoc.com/server/psync.html)
* [SLAVEOF](http://redisdoc.com/server/slaveof.html)
* [SLOWLOG](http://redisdoc.com/server/slowlog.html)
* [SYNC](http://redisdoc.com/server/sync.html)
* [TIME](http://redisdoc.com/server/time.html)


## 隐式实现的命令
* `Transaction`，事务性命令通过 `IRedisClient.BeginTransaction` 来隐士完成。
    * [DISCARD](http://redisdoc.com/transaction/discard.html)
    * [EXEC](http://redisdoc.com/transaction/exec.html)
    * [MULTI](http://redisdoc.com/transaction/multi.html)
* `Connection`
    * [AUTH](http://redisdoc.com/connection/auth.html)：初始化 RedisClient 就应该提供 Redis 的密码，来简化整体的流程。