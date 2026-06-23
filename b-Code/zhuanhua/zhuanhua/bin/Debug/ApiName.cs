public class ApiName
{
    #region 系统资源

    /// <summary>
    /// 该接口用于判断机器人支持哪些功能，以及是否已完成初始化。本文档中的部分接口需要依赖特定的capability才能运行。
    /// </summary>
    public const string capabilities = "/api/core/system/v1/capabilities";

    /// <summary>
    /// 获取机器人电源状态
    /// </summary>
    public const string power_status = "/api/core/system/v1/power/status";

    /// <summary>
    /// 关闭或重启机器人
    /// </summary>
    public const string power_shutdown = "/api/core/system/v1/power/:shutdown";

    /// <summary>
    /// 休眠时激光雷达暂停工作
    /// </summary>
    public const string power_hibernate = "/api/core/system/v1/power/:hibernate";

    /// <summary>
    /// 唤醒机器人
    /// </summary>
    public const string power_wakeup = "/api/core/system/v1/power/:wakeup";

    /// <summary>
    /// 重启模块
    /// </summary>
    public const string power_restartmodule = "/api/core/system/v1/power/:restartmodule";

    /// <summary>
    /// 获取设备信息
    /// </summary>
    public const string robot_info = "/api/core/system/v1/robot/info";

    /// <summary>
    /// 获取设备健康状态信息
    /// </summary>
    public const string robot_health = "/api/core/system/v1/robot/health";

    /// <summary>
    /// 清除出错的状态信息
    /// </summary>
    public const string robot_health_{error_code} = "/api/core/system/v1/robot/health/{error_code}";

    /// <summary>
    /// <h4>所需最低固件版本 4.2.2</h4>
    /// </summary>
    public const string laserscan = "/api/core/system/v1/laserscan";

    /// <summary>
    /// 获取系统参数
    /// </summary>
    public const string parameter = "/api/core/system/v1/parameter";

    /// <summary>
    /// 设置系统参数
    /// </summary>
    public const string parameter = "/api/core/system/v1/parameter";

    /// <summary>
    /// 获取机器人当前的网络状态
    /// </summary>
    public const string network_status = "/api/core/system/v1/network/status";

    /// <summary>
    /// 当网络由安卓管理时，该接口会返回false
    /// </summary>
    public const string network_status = "/api/core/system/v1/network/status";

    /// <summary>
    /// 获取路由信息
    /// </summary>
    public const string network_route = "/api/core/system/v1/network/route";

    /// <summary>
    /// 可设置路由优先级，当wifi和4g都可用时，可选择wifi优先或者4g优先。
    /// </summary>
    public const string network_route = "/api/core/system/v1/network/route";

    /// <summary>
    /// <h4>所需最低固件版本 4.4.0</h4>
    /// </summary>
    public const string network_apn = "/api/core/system/v1/network/apn";

    /// <summary>
    /// 根据地区来设置cmlink apn，设置4g在不同地区的接入点，具体的apn请查阅运营商官网<h4>所需最低固件版本 4.4.0</h4>
    /// </summary>
    public const string network_apn = "/api/core/system/v1/network/apn";

    /// <summary>
    /// 以二进制方式读取cube_cfg_dat文件作为Request Body. </br>Cube配置文件请用RoboStudio的Cube配置工具导出或联系思岚技术支持获取. <h4>所需最低固件版本  4.2.0</h4>
    /// </summary>
    public const string cube_config = "/api/core/system/v1/cube/config";

    /// <summary>
    /// 可以设置不同通道，不同部分，不同类型的led灯颜色效果。
    /// </summary>
    public const string light_control = "/api/core/system/v1/light/control";

    /// <summary>
    /// 设置AEB功能打开或者关闭。
    /// </summary>
    public const string aeb_control = "/api/core/system/v1/aeb/control";

    /// <summary>
    /// 获取千斤顶状态。
    /// </summary>
    public const string jack_status = "/api/core/system/v1/jack/status";

    /// <summary>
    /// 设置千斤顶状态。
    /// </summary>
    public const string jack_status = "/api/core/system/v1/jack/status";

    /// <summary>
    /// 获取机器人IMU的ADC原始值
    /// </summary>
    public const string rawadcimu = "/api/core/system/v1/rawadcimu";

    /// <summary>
    /// 获取电池包的电流和温度
    /// </summary>
    public const string battery_pack = "/api/core/system/v1/battery/pack";

    /// <summary>
    /// 获取机器人IMU原始值
    /// </summary>
    public const string rawimu = "/api/core/system/v1/rawimu";

    #endregion

    #region 定位、建图相关功能

    /// <summary>
    /// 获取机器人位姿
    /// </summary>
    public const string localization_pose = "/api/core/slam/v1/localization/pose";

    /// <summary>
    /// 将机器人强制设置到地图中的某个位置
    /// </summary>
    public const string localization_pose = "/api/core/slam/v1/localization/pose";

    /// <summary>
    /// 获取机器人里程计位姿
    /// </summary>
    public const string localization_odopose = "/api/core/slam/v1/localization/odopose";

    /// <summary>
    /// 定位质量范围 0 ~ 100，值越大表示定位越好
    /// </summary>
    public const string localization_quality = "/api/core/slam/v1/localization/quality";

    /// <summary>
    /// 返回值true表示支持定位，false表示暂停定位即纯里程模式
    /// </summary>
    public const string localization_enable = "/api/core/slam/v1/localization/:enable";

    /// <summary>
    /// 返回值true表示操作成功
    /// </summary>
    public const string localization_enable = "/api/core/slam/v1/localization/:enable";

    /// <summary>
    /// 将定位状态重置
    /// </summary>
    public const string localization_status_reset = "/api/core/slam/v1/localization/status/:reset";

    /// <summary>
    /// 返回值true表示建图模式，false表示定位模式
    /// </summary>
    public const string mapping_enable = "/api/core/slam/v1/mapping/:enable";

    /// <summary>
    /// 返回值true表示操作成功
    /// </summary>
    public const string mapping_enable = "/api/core/slam/v1/mapping/:enable";

    /// <summary>
    /// <h4>所需最低固件版本 4.6.0</h4>
    /// </summary>
    public const string loopclosure_enable = "/api/core/slam/v1/loopclosure/:enable";

    /// <summary>
    /// 返回值true表示操作成功<h4>所需最低固件版本 4.6.0</h4>
    /// </summary>
    public const string loopclosure_enable = "/api/core/slam/v1/loopclosure/:enable";

    /// <summary>
    /// 获取当前的充电桩位置，如果当前地图中不存在充电桩，则返回404错误
    /// </summary>
    public const string homepose = "/api/core/slam/v1/homepose";

    /// <summary>
    /// 设置当前的充电桩位置，当地图中存在多个充电桩时，需要上位机设置其中一个作为当前使用的桩。
    /// </summary>
    public const string homepose = "/api/core/slam/v1/homepose";

    /// <summary>
    /// 获取机器人的所有充电桩信息。<h4>所需最低固件版本 4.3.2</h4>
    /// </summary>
    public const string homedocks = "/api/core/slam/v1/homedocks";

    /// <summary>
    /// 给机器人添加一个充电桩，metadata需要display_name字段，表示充电桩名称。
    /// </summary>
    public const string homedocks = "/api/core/slam/v1/homedocks";

    /// <summary>
    /// 设置机器人的所有充电桩信息。<h4>所需最低固件版本 4.3.2</h4>
    /// </summary>
    public const string homedocks = "/api/core/slam/v1/homedocks";

    /// <summary>
    /// 清空充电桩信息
    /// </summary>
    public const string homedocks = "/api/core/slam/v1/homedocks";

    /// <summary>
    /// 根据机器人当前位置在地图上注册一个充电桩
    /// </summary>
    public const string homedocks_register = "/api/core/slam/v1/homedocks/:register";

    /// <summary>
    /// 编辑充电桩信息，id不可修改，只允许修改pose和metadata
    /// </summary>
    public const string homedocks_{dock_id} = "/api/core/slam/v1/homedocks/{dock_id}";

    /// <summary>
    /// 移除一个充电桩
    /// </summary>
    public const string homedocks_{dock_id} = "/api/core/slam/v1/homedocks/{dock_id}";

    /// <summary>
    /// 获取以机器人坐标系表示的IMU数据
    /// </summary>
    public const string imu = "/api/core/slam/v1/imu";

    /// <summary>
    /// 已知区域即当前地图的范围, 机器人的活动空间和各种人工标记元素都应当在此范围内
    /// </summary>
    public const string knownarea = "/api/core/slam/v1/knownarea";

    /// <summary>
    /// 获取激光探索的栅格地图, 可通过min_x, min_y, max_x, max_y指定获取的范围, 默认获取全部地图. </br> 响应报文为二进制字节流，前32字节为元数据(低位字节在前)，后续为地图数据。 <table border="1" cellspacing='6'><tr><td>位置</td><td>数据类型</td><td>描述</td></tr><tr><td>0-3</td><td>float</td><td>地图起始位置的X坐标</td></tr><tr><td>4-7</td><td>float</td><td>地图起始位置的Y坐标</td></tr><tr><td>8-11</td><td>uint32</td><td>X轴方向栅格数量</td></tr><tr><td>12-15</td><td>uint32</td><td>Y轴方向栅格数量</td></tr><tr><td>16-19</td><td>float</td><td>地图分辨率，每个格子的边长，单位米</td></tr><tr><td>20-31</td><td>byte[]</td><td>预留</td></tr><tr><td>32-35</td><td>uint32</td><td>后续数据的字节数，该值应当等于X轴栅格数*Y轴栅格数</td></tr><tr><td>36-End</td><td>byte[]</td><td>地图数据，每个字节代表一个格子</td></tr></table>
    /// </summary>
    public const string maps_explore = "/api/core/slam/v1/maps/explore";

    /// <summary>
    /// 包含所有数据的复合地图 </br> 响应报文为二进制字节流，可直接保存为stcm文件.
    /// </summary>
    public const string maps_stcm = "/api/core/slam/v1/maps/stcm";

    /// <summary>
    /// 将地图设置到slamware系统中, 以二进制方式读取stcm文件作为request body。</br>机器人位姿会被重置到原点，需要重新设置机器人位姿.<br/> 【注意】地图不会持久化保存，重启后即失效
    /// </summary>
    public const string maps_stcm = "/api/core/slam/v1/maps/stcm";

    /// <summary>
    /// 清空地图
    /// </summary>
    public const string maps = "/api/core/slam/v1/maps";

    /// <summary>
    /// 移动地图原点,并更新到slamware系统中
    /// </summary>
    public const string maps_origin = "/api/core/slam/v1/maps/origin";

    #endregion

    #region 地图语义元素

    /// <summary>
    /// 获取虚拟线段
    /// </summary>
    public const string lines_{usage} = "/api/core/artifact/v1/lines/{usage}";

    /// <summary>
    /// 添加时id为无效字段，可为任意值。
    /// </summary>
    public const string lines_{usage} = "/api/core/artifact/v1/lines/{usage}";

    /// <summary>
    /// 修改虚拟线段
    /// </summary>
    public const string lines_{usage} = "/api/core/artifact/v1/lines/{usage}";

    /// <summary>
    /// 清空某一类虚拟线段
    /// </summary>
    public const string lines_{usage} = "/api/core/artifact/v1/lines/{usage}";

    /// <summary>
    /// 删除虚拟线段
    /// </summary>
    public const string lines_{usage}_{id} = "/api/core/artifact/v1/lines/{usage}/{id}";

    /// <summary>
    /// 获取矩形区域
    /// </summary>
    public const string rectangle_areas_{usage} = "/api/core/artifact/v1/rectangle-areas/{usage}";

    /// <summary>
    /// 不同类型的矩形区域，所需要的metadata也不同，请参考文档描述。
    /// </summary>
    public const string rectangle_areas_{usage} = "/api/core/artifact/v1/rectangle-areas/{usage}";

    /// <summary>
    /// 清空某一类矩形区域
    /// </summary>
    public const string rectangle_areas_{usage} = "/api/core/artifact/v1/rectangle-areas/{usage}";

    /// <summary>
    /// 修改指定ID的矩形区域坐标或metadata。
    /// </summary>
    public const string rectangle_areas_{usage}_{id} = "/api/core/artifact/v1/rectangle-areas/{usage}/{id}";

    /// <summary>
    /// 删除矩形区域
    /// </summary>
    public const string rectangle_areas_{usage}_{id} = "/api/core/artifact/v1/rectangle-areas/{usage}/{id}";

    /// <summary>
    /// POI指Point of interest, 也称为星标或兴趣点，用于标记地图上的某个位姿，以及若干与业务逻辑相关的metadata。
    /// </summary>
    public const string pois = "/api/core/artifact/v1/pois";

    /// <summary>
    /// 调用方应当随机生成一个UUID作为id, metadata中的display_name用于界面显示, type用于区分POI类型。<br/> 在建图过程中添加POI时，建议不包含Pose，此时会用机器人当前位置创建POI，并且记录传感器观测信息，在闭环后会进行位姿调整。
    /// </summary>
    public const string pois = "/api/core/artifact/v1/pois";

    /// <summary>
    /// 清空POI
    /// </summary>
    public const string pois = "/api/core/artifact/v1/pois";

    /// <summary>
    /// 如果在建图时添加POI，则在闭环后POI会跟着调整位姿，调用该接口可以进一步减少位姿调整的误差。<br/> 【注意】仅在闭环后调用有效，其他时候无需调用。<h4>所需最低固件版本  4.2.4</h4>
    /// </summary>
    public const string pois_adjust = "/api/core/artifact/v1/pois/:adjust";

    /// <summary>
    /// 根据ID查找POI
    /// </summary>
    public const string pois_{poi_id} = "/api/core/artifact/v1/pois/{poi_id}";

    /// <summary>
    /// 请求报文中pose和metadata可以只包含其中一个，则另一个字段保持不变。
    /// </summary>
    public const string pois_{poi_id} = "/api/core/artifact/v1/pois/{poi_id}";

    /// <summary>
    /// 删除POI
    /// </summary>
    public const string pois_{poi_id} = "/api/core/artifact/v1/pois/{poi_id}";

    /// <summary>
    /// 激光地标指激光雷达识别到的反光板位置。<h4>所需最低固件版本：5.1.1</h4>
    /// </summary>
    public const string laser_landmarks = "/api/core/artifact/v1/laser-landmarks";

    /// <summary>
    /// 将从地图中读出的激光地标信息设置到Slamware中<h4>所需最低固件版本：5.1.1</h4>
    /// </summary>
    public const string laser_landmarks = "/api/core/artifact/v1/laser-landmarks";

    /// <summary>
    /// 清空所有激光地标<h4>所需最低固件版本：5.1.1</h4>
    /// </summary>
    public const string laser_landmarks = "/api/core/artifact/v1/laser-landmarks";

    /// <summary>
    /// Slamware是否正在自动更新激光地标<h4>所需最低固件版本：5.1.1</h4>
    /// </summary>
    public const string laser_landmarks_update = "/api/core/artifact/v1/laser-landmarks/:update";

    /// <summary>
    /// 设置是否允许Slamware自动更新激光地标<h4>所需最低固件版本：5.1.1</h4>
    /// </summary>
    public const string laser_landmarks_update = "/api/core/artifact/v1/laser-landmarks/:update";

    /// <summary>
    /// 删除部分激光地标, 请求报文为ID数组，ID来自获取激光地标接口返回内容的id字段。<h4>所需最低固件版本：5.1.1</h4>
    /// </summary>
    public const string laser_landmarks_remove = "/api/core/artifact/v1/laser-landmarks/:remove";

    #endregion

    #region 机器人运动控制

    /// <summary>
    /// 获取所有支持的Action
    /// </summary>
    public const string action_factories = "/api/core/motion/v1/action-factories";

    /// <summary>
    /// 获取当前行为
    /// </summary>
    public const string actions_current = "/api/core/motion/v1/actions/:current";

    /// <summary>
    /// 终止当前行为
    /// </summary>
    public const string actions_current = "/api/core/motion/v1/actions/:current";

    /// <summary>
    /// 创建新的运动行为
    /// </summary>
    public const string actions = "/api/core/motion/v1/actions";

    /// <summary>
    /// 可查询最近20次action的状态, state.status为4表示action已结束，此时通过result判断成功与否。
    /// </summary>
    public const string actions_{action_id} = "/api/core/motion/v1/actions/{action_id}";

    /// <summary>
    /// 当前Action剩余的路径点
    /// </summary>
    public const string path = "/api/core/motion/v1/path";

    /// <summary>
    /// 当前Action剩余的目标点
    /// </summary>
    public const string milestones = "/api/core/motion/v1/milestones";

    /// <summary>
    /// 获取机器人当前运动速度
    /// </summary>
    public const string speed = "/api/core/motion/v1/speed";

    /// <summary>
    /// 获取机器人到目的地的剩余运动时间（估计值）
    /// </summary>
    public const string time = "/api/core/motion/v1/time";

    /// <summary>
    /// 搜索从机器人到目标点的最优路径
    /// </summary>
    public const string search_path = "/api/core/motion/v1/:search_path";

    /// <summary>
    /// 运动策略为Slamware一系列内部参数的组合，涉及到运动速度、避障行为等各个方面，不同的策略可适用于不同的场景。一般情况下采用默认策略即可。<h4>所需最低固件版本 4.2.4</h4>
    /// </summary>
    public const string strategies = "/api/core/motion/v1/strategies";

    /// <summary>
    /// 获取当前运动策略
    /// </summary>
    public const string strategies_current = "/api/core/motion/v1/strategies/:current";

    /// <summary>
    /// 设置运动策略
    /// </summary>
    public const string strategies_current = "/api/core/motion/v1/strategies/:current";

    #endregion

    #region 固件升级

    /// <summary>
    /// 从云端查询可升级的新版本固件信息，如果没有则返回空数据
    /// </summary>
    public const string newversion = "/api/core/firmware/v1/newversion";

    /// <summary>
    /// 上传固件到思岚云并发布给指定设备，如果设备支持自动升级，就会在指定时间段内自动升级固件。
    /// </summary>
    public const string autoupdate_enable = "/api/core/firmware/v1/autoupdate/:enable";

    /// <summary>
    /// 关闭自动升级后将会忽略云端发布的最新固件。
    /// </summary>
    public const string autoupdate_enable = "/api/core/firmware/v1/autoupdate/:enable";

    /// <summary>
    /// 查询思岚云上可升级的最新固件，下载固件并升级。
    /// </summary>
    public const string autoupdate_start = "/api/core/firmware/v1/autoupdate/:start";

    /// <summary>
    /// 将固件包以二进制方式读取作为request body，上传至机器人用于固件升级。<h4>所需最低固件版本：4.6.3</h4>
    /// </summary>
    public const string update_start = "/api/core/firmware/v1/update/:start";

    /// <summary>
    /// 获取固件升级进度
    /// </summary>
    public const string progress = "/api/core/firmware/v1/progress";

    #endregion

    #region 运行数据统计

    /// <summary>
    /// 机器人总的运行里程，单位米
    /// </summary>
    public const string odometry = "/api/core/statistics/v1/odometry";

    /// <summary>
    /// 机器人总的运行时间，单位秒
    /// </summary>
    public const string runtime = "/api/core/statistics/v1/runtime";

    #endregion

    #region 传感器控制

    /// <summary>
    /// 用户设置是否使用深度摄像头数据
    /// </summary>
    public const string depth_enable = "/api/core/sensors/v1/depth/:enable";

    /// <summary>
    /// 获取禁用状态的传感器掩码信息。
    /// </summary>
    public const string masks = "/api/core/sensors/v1/masks";

    /// <summary>
    /// 设置传感器掩码。
    /// </summary>
    public const string masks = "/api/core/sensors/v1/masks";

    #endregion

    #region 安卓应用程序管理(仅限ARM平台)

    /// <summary>
    /// 获取所有自定义安装的APP
    /// </summary>
    public const string apps = "/api/core/application/v1/apps";

    /// <summary>
    /// 安装APP
    /// </summary>
    public const string apps = "/api/core/application/v1/apps";

    /// <summary>
    /// 卸载一个APP
    /// </summary>
    public const string apps_{app_name} = "/api/core/application/v1/apps/{app_name}";

    #endregion

    #region 机器人通用底盘和平台相关的功能

    /// <summary>
    /// 获取系统启动以来的毫秒数, 返回值为字符串格式的整数。<h4>所需最低固件版本 4.2.4</h4>
    /// </summary>
    public const string api_platform_v1_timestamp = "/api/platform/v1/timestamp";

    /// <summary>
    /// 获取机器人发生的事件，上位机可以播报语音或进行别的交互，启用不同的插件会扩展出不同的事件类型。
    /// </summary>
    public const string api_platform_v1_events = "/api/platform/v1/events";

    #endregion

    #region 多楼层地图管理，乘电梯等功能

    /// <summary>
    /// 获取地图状态信息
    /// </summary>
    public const string api_multi_floor_status = "/api/multi-floor/status";

    /// <summary>
    /// 获取所有楼层信息
    /// </summary>
    public const string api_multi_floor_map_v1_floors = "/api/multi-floor/map/v1/floors";

    /// <summary>
    /// 获取机器人所在楼层信息
    /// </summary>
    public const string api_multi_floor_map_v1_floors_current = "/api/multi-floor/map/v1/floors/:current";

    /// <summary>
    /// 正常情况下应当由机器人在乘坐电梯过程中自主切换楼层，该接口仅供特殊情况下（如人工搬运机器人）使用。
    /// </summary>
    public const string api_multi_floor_map_v1_floors_current = "/api/multi-floor/map/v1/floors/:current";

    /// <summary>
    /// 通过参数指定楼层，不带参数时获取所有楼层的POI。
    /// </summary>
    public const string api_multi_floor_map_v1_pois = "/api/multi-floor/map/v1/pois";

    /// <summary>
    /// 查找离机器人最近的POI信息。其中name有三个特殊值，ON_DOCK表示在桩上，IN_ELEVATOR表示在电梯内，UNKNOWN表示没有POI，此时没有relative_pose字段，其他的值均表示地图中添加的常规POI的名称。
    /// </summary>
    public const string api_multi_floor_map_v1_pois_search_nearby = "/api/multi-floor/map/v1/pois/:search_nearby";

    /// <summary>
    /// 给定若干个POI名称，返回调整顺序后的POI名称，使得机器人依次遍历这些POI并回到当前位置的总路径最短。</br>【注】该接口耗时随着POI数量指数增长，请勿传入大量POI。<h4>所需最低固件版本 4.5.0</h4>
    /// </summary>
    public const string api_multi_floor_map_v1_pois_dispatch = "/api/multi-floor/map/v1/pois/:dispatch";

    /// <summary>
    /// 通过Query参数指定楼层，不带参数时获取所有楼层的充电桩
    /// </summary>
    public const string api_multi_floor_map_v1_homedocks = "/api/multi-floor/map/v1/homedocks";

    /// <summary>
    /// 获取机器人当前绑定的充电桩信息，如果没绑定过或dock id无效，返回的result为false。
    /// </summary>
    public const string api_multi_floor_map_v1_homedocks_current = "/api/multi-floor/map/v1/homedocks/:current";

    /// <summary>
    /// 【注意】如果绑定的充电桩不在启动楼层，则需要先将机器人推到充电桩上，然后调用本接口，此时会同步修改启动楼层并重置地图。
    /// </summary>
    public const string api_multi_floor_map_v1_homedocks_current = "/api/multi-floor/map/v1/homedocks/:current";

    /// <summary>
    /// 调用该接口前请确保机器人定位准确。
    /// </summary>
    public const string api_multi_floor_map_v1_homedocks_search_nearby = "/api/multi-floor/map/v1/homedocks/:search_nearby";

    /// <summary>
    /// 上传的地图会持久化保存在文件系统中, 但不会加载到Slamware中。<br/> 【注意】当机器人由云端管理时，从云端下载的地图会覆盖本地地图。
    /// </summary>
    public const string api_multi_floor_map_v1_stcm = "/api/multi-floor/map/v1/stcm";

    /// <summary>
    /// 不会清空内存中的当前地图，而是删除文件系统中缓存的地图
    /// </summary>
    public const string api_multi_floor_map_v1_stcm = "/api/multi-floor/map/v1/stcm";

    /// <summary>
    /// 从Slamware中读取地图并保存到文件。<br/> 【注意】 多楼层环境中禁止该操作，否则会丢失其他楼层的地图。
    /// </summary>
    public const string api_multi_floor_map_v1_stcm_save = "/api/multi-floor/map/v1/stcm/:save";

    /// <summary>
    /// 重新加载地图，优先尝试从云端下载，下载失败或机器人不受云端管理时从本地文件读取。<br/> pose为可选字段，pose为空时设置机器人位姿到充电桩前。 <br/>【注意】系统启动时会自动加载地图，该接口一般在部署阶段地图有变更时才需要调用。
    /// </summary>
    public const string api_multi_floor_map_v1_stcm_reload = "/api/multi-floor/map/v1/stcm/:reload";

    /// <summary>
    /// 保存当前地图到文件，并重新加载，相当于save和reload 2个接口的组合。<br/>【注意】 多楼层环境中禁止该操作，否则会丢失其他楼层的地图。<h4>所需最低固件版本  4.2.4</h4>
    /// </summary>
    public const string api_multi_floor_map_v1_stcm_sync = "/api/multi-floor/map/v1/stcm/:sync";

    /// <summary>
    /// 将机器人与云端场景解除绑定，并删除本地地图，在机器人需要换场景部署时调用。<h4>所需最低固件版本  6.2.0</h4>
    /// </summary>
    public const string api_multi_floor_map_v1_scene_unbind = "/api/multi-floor/map/v1/scene/unbind";

    /// <summary>
    /// 在轨道构成的图中，搜索起点到终点的可行路径。
    /// </summary>
    public const string api_multi_floor_map_v1_search_path_points = "/api/multi-floor/map/v1/search_path_points";

    /// <summary>
    /// 将机器人位姿设置到指定的POI上，一般用于发生异常后的恢复操作。<h4>所需最低固件版本 4.5.3</h4>
    /// </summary>
    public const string api_multi_floor_localization_v1_pose = "/api/multi-floor/localization/v1/pose";

    /// <summary>
    /// 将机器人位姿设置到指定的充电桩前，一般用于发生异常后的恢复操作。<h4>所需最低固件版本 6.2.0</h4>
    /// </summary>
    public const string api_multi_floor_localization_v1_homedock = "/api/multi-floor/localization/v1/homedock";

    /// <summary>
    /// 获取电梯区域内的元素，包括电梯ID以及等待点。
    /// </summary>
    public const string api_multi_floor_map_v1_elevators = "/api/multi-floor/map/v1/elevators";

    /// <summary>
    /// 获取某个电梯的信息
    /// </summary>
    public const string api_multi_floor_map_v1_elevators_{elevator_id} = "/api/multi-floor/map/v1/elevators/{elevator_id}";

    /// <summary>
    /// 获取机器人与电梯的位置关系
    /// </summary>
    public const string api_multi_floor_map_v1_elevators_{elevator_id}_pose_relation = "/api/multi-floor/map/v1/elevators/{elevator_id}/pose_relation";

    #endregion

    #region 工业搬运服务

    /// <summary>
    /// 获取当前设备所属场景下的所有任务模板
    /// </summary>
    public const string api_industry_v1_tasks_templates = "/api/industry/v1/tasks/templates";

    /// <summary>
    /// 创建一个呼叫器任务模板
    /// </summary>
    public const string api_industry_v1_tasks_templates = "/api/industry/v1/tasks/templates";

    /// <summary>
    /// 删除一个任务模板
    /// </summary>
    public const string api_industry_v1_tasks_templates_{key_id} = "/api/industry/v1/tasks/templates/{key_id}";

    /// <summary>
    /// 默认返回ready和running状态的所有类型的任务，status为all时表示查询最近的所有任务，包括已成功完成和失败的任务。
    /// </summary>
    public const string api_industry_v1_tasks = "/api/industry/v1/tasks";

    /// <summary>
    /// 上位机执行呼叫器任务时，通过该接口推送任务事件，同时更新任务状态。
    /// </summary>
    public const string api_industry_v1_tasks_events = "/api/industry/v1/tasks/events";

    #endregion

    #region 配送服务(仅限整机，通用底盘无法支持)

    /// <summary>
    /// expires表示密码过期时间，如果不包含这个字段则意味着密码永久有效，enable表示是否启用操作密码
    /// </summary>
    public const string api_delivery_v1_admin_password = "/api/delivery/v1/admin/password";

    /// <summary>
    /// 如果enable为false，则表示禁用密码
    /// </summary>
    public const string api_delivery_v1_admin_password = "/api/delivery/v1/admin/password";

    /// <summary>
    /// 获取机器人工作模式
    /// </summary>
    public const string api_delivery_v1_admin_mode = "/api/delivery/v1/admin/mode";

    /// <summary>
    /// 设置机器人工作模式
    /// </summary>
    public const string api_delivery_v1_admin_mode = "/api/delivery/v1/admin/mode";

    /// <summary>
    /// <h4>所需最低固件版本 4.3.2</h4>
    /// </summary>
    public const string api_delivery_v1_admin_language = "/api/delivery/v1/admin/language";

    /// <summary>
    /// <h4>所需最低固件版本 4.3.2</h4>
    /// </summary>
    public const string api_delivery_v1_admin_language = "/api/delivery/v1/admin/language";

    /// <summary>
    /// <h4>所需最低固件版本 4.3.3</h4>
    /// </summary>
    public const string api_delivery_v1_admin_working_time = "/api/delivery/v1/admin/working_time";

    /// <summary>
    /// <h4>所需最低固件版本 4.3.3</h4>
    /// </summary>
    public const string api_delivery_v1_admin_working_time = "/api/delivery/v1/admin/working_time";

    /// <summary>
    /// 获取运动选项
    /// </summary>
    public const string api_delivery_v1_admin_move_options = "/api/delivery/v1/admin/move_options";

    /// <summary>
    /// 设置在配送过程中采用的运动选项，比如采用自由导航还是轨道模式。当请求报文为空时表示删除已设置的内容，恢复默认选项。不需要包含包含所有字段，按需设置即可。
    /// </summary>
    public const string api_delivery_v1_admin_move_options = "/api/delivery/v1/admin/move_options";

    /// <summary>
    /// <h4>所需最低固件版本 4.5.3</h4>
    /// </summary>
    public const string api_delivery_v1_admin_line_speed = "/api/delivery/v1/admin/line_speed";

    /// <summary>
    /// <h4>所需最低固件版本 4.5.3</h4>
    /// </summary>
    public const string api_delivery_v1_admin_line_speed = "/api/delivery/v1/admin/line_speed";

    /// <summary>
    /// 获取机器配置信息
    /// </summary>
    public const string api_delivery_v1_configurations = "/api/delivery/v1/configurations";

    /// <summary>
    /// 获取配送相关的设置信息
    /// </summary>
    public const string api_delivery_v1_settings = "/api/delivery/v1/settings";

    /// <summary>
    /// 设置任务的超时时间
    /// </summary>
    public const string api_delivery_v1_settings_timeout = "/api/delivery/v1/settings/timeout";

    /// <summary>
    /// 从云端获取语音包信息，网络不好时该接口可能耗时较久。<h4>所需最低固件版本 4.3.2</h4>
    /// </summary>
    public const string api_delivery_v1_voice_resources = "/api/delivery/v1/voice_resources";

    /// <summary>
    /// 只有带货仓的机型支持cargos系列接口
    /// </summary>
    public const string api_delivery_v1_cargos = "/api/delivery/v1/cargos";

    /// <summary>
    /// 获取某个Cargo所有Box信息
    /// </summary>
    public const string api_delivery_v1_cargos_{cargo_id}_boxes = "/api/delivery/v1/cargos/{cargo_id}/boxes";

    /// <summary>
    /// 获取Box信息
    /// </summary>
    public const string api_delivery_v1_cargos_{cargo_id}_boxes_{box_id} = "/api/delivery/v1/cargos/{cargo_id}/boxes/{box_id}";

    /// <summary>
    /// 操作Box
    /// </summary>
    public const string api_delivery_v1_cargos_{cargo_id}_boxes_{box_id}_{op} = "/api/delivery/v1/cargos/{cargo_id}/boxes/{box_id}/{op}";

    /// <summary>
    /// 查询Box操作结果
    /// </summary>
    public const string api_delivery_v1_cargos_{cargo_id}_boxes_{box_id}_operation_result = "/api/delivery/v1/cargos/{cargo_id}/boxes/{box_id}/operation_result";

    /// <summary>
    /// 获取被占用的外卖舱
    /// </summary>
    public const string api_delivery_v1_cargos_assigned = "/api/delivery/v1/cargos/assigned";

    /// <summary>
    /// 默认返回ready和running状态的所有类型的任务，status为all时表示查询最近的所有任务，包括已成功完成和失败的任务。
    /// </summary>
    public const string api_delivery_v1_tasks = "/api/delivery/v1/tasks";

    /// <summary>
    /// 创建任务
    /// </summary>
    public const string api_delivery_v1_tasks = "/api/delivery/v1/tasks";

    /// <summary>
    /// 取消所有任务
    /// </summary>
    public const string api_delivery_v1_tasks = "/api/delivery/v1/tasks";

    /// <summary>
    /// 一次性创建多个任务 <h4>所需最低固件版本 4.3.0</h4>
    /// </summary>
    public const string api_delivery_v1_tasks_batch = "/api/delivery/v1/tasks/:batch";

    /// <summary>
    /// 有些任务是通过云端下发的，可能不存在订单号，因此需要通过Task ID来取消
    /// </summary>
    public const string api_delivery_v1_tasks_{task_id} = "/api/delivery/v1/tasks/{task_id}";

    /// <summary>
    /// 在机器人端创建的任务，都会包含订单号，因此可以通过订单号取消任务
    /// </summary>
    public const string api_delivery_v1_tasks_orders_{order_id} = "/api/delivery/v1/tasks/orders/{order_id}";

    /// <summary>
    /// * `DEVICE_ERROR` 设备故障，底盘上报了Error信息，机器人无法移动，上位机应当显示故障页面。
    /// * `GOING_TO_TASK_POINT` 正在前往任务点，有些任务（如回盘、取物配送）需要中途停靠某些任务点，完成操作后再前往目标点。
    /// * `ARRIVED_AT_TASK_POINT` 到达任务点，机器人会等待操作完成或超时后再继续下一阶段。
    /// * `ON_DELIVERING` 正在前往目标点，为了兼容采用该名称，实际不一定是配送任务。
    /// * `ARRIVED_AT_TARGET` 到达最终目标点。
    /// * `ON_RETURNING` 正在返航，当机器人有默认停靠点时，该状态表示机器人正在前往该停靠点。
    /// * `GOING_HOME`  正在回桩。
    /// * `IDLE` 空闲，机器人在默认停靠点或桩上时处于该状态。
    /// </summary>
    public const string api_delivery_v1_stage = "/api/delivery/v1/stage";

    /// <summary>
    /// 当用户操作APP时，设为false来禁止机器人移动，此时机器人即使收到任务也不会运行；用户完成操作时，设为true允许机器人运动，此时机器人有任务则执行任务，没任务则回桩或回到类型为PARKING的POI
    /// </summary>
    public const string api_delivery_v1_tasks_task_execution = "/api/delivery/v1/tasks/:task_execution";

    /// <summary>
    /// 与Delete接口的区别是本接口以成功状态结束所有任务。
    /// </summary>
    public const string api_delivery_v1_tasks_task_finish = "/api/delivery/v1/tasks/:task_finish";

    /// <summary>
    /// 通知机器人用户开始取物，一般用于带舱体的机器人，在该接口后再调用开舱指令进行取物，完成后调用end_pickup，如果任务包含多个舱体，则此时自动打开下一个舱门，上位机需要多次调用end_pickup。
    /// </summary>
    public const string api_delivery_v1_tasks_start_pickup = "/api/delivery/v1/tasks/:start_pickup";

    /// <summary>
    /// 通知机器人用户已完成取物。
    /// </summary>
    public const string api_delivery_v1_tasks_end_pickup = "/api/delivery/v1/tasks/:end_pickup";

    /// <summary>
    /// 机器人到达任务点时，该接口用于通知机器人用户已完成操作，可以继续执行任务。
    /// </summary>
    public const string api_delivery_v1_tasks_end_operation = "/api/delivery/v1/tasks/:end_operation";

    #endregion

}
