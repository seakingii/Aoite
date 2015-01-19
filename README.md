#Aoite(Any one item!)
一个适于任何 .Net Framework 4.0+ 项目的快速开发整体解决方案。

#介绍
本项目从2009年孵化（V->Sofire->Aoite），至今已度过5个年头。一直在优化，一直在重构，一直在**商用**。有十分完整的单元测试用例。可以放心使用（我吹牛了，请暂时不要商用，目前开源版还未彻底完成所有功能，请等到 CommandModel 模块完成。）。更多内容请关注我的[博客园](http://www.cnblogs.com/sofire)。

#Project Plan （2015-01-19 ~ 2015-01-24）
1. 完成 Redis 的 95%+ 命令。
	* 考虑实现基于面向对象扩展方式。
	* RealCall 单元测试前期可能不会实现所有命令。
2. 完成 Cache 模块。
3. 完成 CommandModel 模块（这个模块是 Aoite 最大亮点之一，暂时保密用途）。
4. 完成 ASP.NET MVC CommandModel 模块。
5. 编写文档（2015-01-24 以后，从博客园首发）。

#已完成重要模块介绍
##Aoite
1. Aoite.Data：数据库交互模块。你从未有过的数据库连接体验方式。
2. Aoite.LevelDB：Google LevelDB 封装。需要的人很需要，不需要的人可以略过。
3. Aoite.Logger：日志模块。小巧易用好扩展，居家旅行，排查解读。
4. Aoite.Net：其实这块以前费了很大心思（以前的Sofire版就是这样的），但是由于存在内存泄漏，这次的重构，暂时不放出来。
5. Aoite.Reflection：感谢[Fasterflect](http://fasterflect.codeplex.com/)。版权归其所有。
6. Aoite.Serialization：一个快速的二进制序列化器。

##System
1. System.Mapping：绝对干货。快速反射。
2. System.IOC：智能 Ioc 模式。从此告别依赖。
3. System.Random：最好用的随机模块。

## 更多内容
在文档尚未撰写完毕之前，你可以通过单元测试了解整个框架。欢迎批评指导~
