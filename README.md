# HapyListen（哈皮）
一个能听歌的聊天室，设计思路源自以前某位大佬开发的网站（BBBug），但是在原有的设计上做了调整，整体业务上包括：用户模块、音乐模块、聊天室模块三大板块，其中再由三大板块衍生出新的业务；

**申明：本软件只作为免费学习使用，不涉及任何商业应用，任何关于收费的需求皆与本软件无关！！**

**再次申明：本软件只做流媒体的转发，不涉及任何流媒体资源的存储！**





核心模块：
![image-20240810191559347](https://github.com/hapyListen/listen-client-windows/blob/master/pic/image-20240810191559347.png)

用户模块：

![image-20240810191933861](https://github.com/hapyListen/listen-client-windows/blob/master/pic/image-20240810191933861.png)

聊天室模块：

![image-20240810192149105](https://github.com/hapyListen/listen-client-windows/blob/master/pic/image-20240810192149105.png)

音乐模块：

![image-20240810192304268](https://github.com/hapyListen/listen-client-windows/blob/master/pic/image-20240810192304268.png)


## 框架
- .Net Core 8.0
- Avalonia 11.1.2 
- Avalonia.Gif
- AsyncImageLoader.Avalonia
- FFmpeg.AutoGen 4.4.1.1
- CommunityToolKit.Mvvm
- NAudio 2.2.1

---



## 流媒体服务器

[ZLMediaKit](https://github.com/ZLMediaKit/ZLMediaKit)



## 关于第三方音乐接口
- [MyFreeMp3](https://tool.liumingye.cn/music/#/artist/N1BG)
- [TuneFree](https://pt.sayqz.com/#/)



说明：这两个接口都能获取和播放歌曲，但是TuneFree在使用前需要使用网易云登录才能听歌；后面会逐步新增其他音乐API，并且以官方API为准，当然需要有VIP的用户登录然后创建一个听歌房间，再由其他成员点播歌曲；




## 表情包素材地址

https://www.iconfont.cn/collections/detail?spm=a313x.collections_index.i1.d9df05512.2fc03a81eyYlDV&cid=4116

