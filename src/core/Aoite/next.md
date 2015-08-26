#版本更新特性 v3

1. 去处所有 Result 作为返回值，改为直接抛出错误。
2. 增加配置文件读取类、JSON解析类、[JsonConfig](https://github.com/Dynalon/JsonConfig)。
3. 从旧版增加 Remoting


2.0.0.16

1. 修复 Ioc 模块。满足以下条件将会出现 BUG：
    1. 预期类型含有构造函数，且参数大于 0。
    2. 构造函数的参数是一个预期类型，且此接口具有 [DefaultMappingAttribute] 特性。
    3. 当手工对此参数进行映射时，返回的还是 [DefaultMappingAttribute] 特性的默认类型。
    4. 修复结果：事件映射级别最高，手工映射其次
2. 优化 Logger 模块。当日志项长度为 0 时，不执行任何操作。