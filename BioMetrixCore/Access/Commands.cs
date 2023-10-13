using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Access
{
    /// <summary>Command Content
    /// </summary>
    class Commands
    {

        //Update
        //用户信息 下发控制器用户请用下条命令
        public const string Command_UpdateUserInfo = "DATA UPDATE user CardNo={0}\tPin={1}\tPassword={2}\tGroup={3}\tStartTime={4}\tEndTime={5}\tName={6}\tPrivilege={7}";
        //控制器用户信息 
        public const string Command_UpdateUserInfoForInbio = "DATA UPDATE user CardNo={0}\tPin={1}\tPassword={2}\tGroup={3}\tStartTime={4}\tEndTime={5}\tName={6}\tSuperAuthorize={7}\tDisable={8}";
        //拓展用户信息
        public const string Command_UpdateExUserInfo = "DATA UPDATE extuser Pin={0}\tFunSwitch={1}\tFirstName={2}\tLastName={3}\tPersonalVS={4}";
        //一人多卡信息
        public const string Command_UpdateMulCardUser = "DATA UPDATE mulcarduser Pin={0}\tCardNo={1}\tLossCardFlag={2}\tCardType={3}";
        //用户门禁权限
        public const string Command_UpdateUserAuthorize = "DATA UPDATE userauthorize Pin={0}\tAuthorizeTimezoneId={1}\tAuthorizeDoorId={2}";
        //节假日
        public const string Command_UpdateHoliday = "DATA UPDATE holiday Holiday={0}\tHolidayType={1}\tLoop={2}";
        //时间规则
        public const string Command_UpdateTimeZone = "DATA UPDATE timezone TimezoneId={0}\tSunTime1={1}\tSunTime2={2}\tSunTime3={3}\tMonTime1={4}\tMonTime2={5}\tMonTime3={6}\tTueTime1={7}\tTueTime2={8}\tTueTime3={9}\tWedTime1={10}\tWedTime2={11}\tWedTime3={12}\tThuTime1={13}\tThuTime2={14}\tThuTime3={15}\tFriTime1={16}\tFriTime2={17}\tFriTime3={18}\tSatTime1={19}\tSatTime2={20}\tSatTime3={21}\tHol1Time1={22}\tHol1Time2={23}\tHol1Time3={24}\tHol2Time1={25}\tHol2Time2={26}\tHol2Time3={27}\tHol3Time1={28}\tHol3Time2={29}\tHol3Time3={30}";
        //指纹模板
        public const string Command_UpdateTemplatev10 = "DATA UPDATE templatev10 Pin={0}\tFingerID={1}\tValid={2}\tTemplate={3}";
        //人脸模板
        public const string Command_UpdateFaceTmp = "DATA UPDATE facev7 Pin={0}\tFaceID={1}\tSize={2}\tValid={3}\tFace={4}";
        //首卡开门
        public const string Command_UpdateFirstCard = "DATA UPDATE firstcard DoorID={0}\tTimezoneID={1}\tPin={2}";
        //多卡开门组合信息
        public const string Command_UpdateMultimCard = "DATA UPDATE multimcard Index={0}\tDoorId={1}\tGroup1={2}\tGroup2={3}\tGroup3={4}\tGroup4={5}\tGroup5={6}";
        //联动详细信息
        public const string Command_UpdateInOutFun = "DATA UPDATE inoutfun Index={0}\tEventType={1}\tInAddr={2}\tOutType={3}\tOutAddr={4}\tOutTime={5}\tReserved={6}";
        //定时输出信息
        public const string Command_UpdateOutRelaySetting = "DATA UPDATE outrelaysetting Num={0}\tOutType={1}\tActionType={2}\tTimezoneId={3}";
        //夏令时信息 SET OPTIONS DSTOn=1,DLSTMode=1
        public const string Command_UpdateDSTSetting = "DATA UPDATE DSTSetting Year={0}\tStartTime={1}\tEndTime={2}\tLoop={3}";
        //设备属性信息
        public const string Command_UpdateDevProperty = "DATA UPDATE DevProperty ID={0}\tType={1}\tBusType={2}\tMachineType={3}\tAddress={4}\tMac={5}\tIPAddress={6}\tSN={7}\tIsMaster={8}";
        //设备参数信息
        public const string Command_UpdateDevParameters = "DATA UPDATE DevParameters ID={0}\tName={1}\tValue={2}";
        //门属性信息
        public const string Command_UpdateDoorProperty = "DATA UPDATE DoorProperty ID={0}\tDevID={1}\tAddress={2}\tDisable={3}";
        //门参数信息
        public const string Command_UpdateDoorParameters = "DATA UPDATE DoorParameters ID={0}\tName={1}\tValue={2}\tDevID={3}";
        //读头属性信息
        public const string Command_UpdateReaderProperty = "DATA UPDATE ReaderProperty ID={0}\tDoorID={1}\tType={2}\tAddress={3}\tIPAddress={4}\tPort={5}\tMAC={6}\tInOut={7}\tDisable={8}\tVerifyType={9}\tMulticast={10}\tDevID={11}\tOfflineRefuse={12}";
        //堵头参数信息
        public const string Command_UpdateReaderParameters = "DATA UPDATE ReaderParameters ID={0}\tDevID={1}\tName={2}\tValue={3}";
        //辅助输入属性信息
        public const string Command_UpdateAuxIn = "DATA UPDATE AuxIn ID={0}\tDevID={1}\tAddress={2}\tDisable={3}";
        //辅助输出属性信息
        public const string Command_UpdateAuxOut = "DATA UPDATE AuxOut ID={0}\tDevID={1}\tAddress={2}\tDisable={3}";
        //默认韦根格式信息
        public const string Command_UpdateDefaultWGFormat = "DATA UPDATE DefaultWGFormat ID={0}\tCardBit={1}\tSiteCode={2}\tFormatName={3}\tCardFormat={4}";
        //韦根格式信息
        public const string Command_UpdateWGFormat = "DATA UPDATE WGFormat ID={0}\tCardBit={1}\tSiteCode={2}\tFormatName={3}\tCardFormat={4}";
        //韦根读头对应的韦根格式信息
        public const string Command_UpdateReaderWGFormat = "DATA UPDATE ReaderWGFormat ReaderID={0}\tDevID={1}\tWGFormatID={2}\tParityVerifyDisable={3}\tReversalType={4}";
        //输入控制（受时间段限制）信息
        public const string Command_UpdateInputIOSetting = "DATA UPDATE InputIOSetting Number={0}\tInType={1}\tTimeZoneId={2}\tDevID={3}";
        //不同时段的验证方式信息
        public const string Command_UpdateDiffTimezoneVS = @"DATA UPDATE DiffTimezoneVS TimezoneID={0}\tSunTime1={1}\tSunTime1VSUser={2}\tSunTime1VSDoor={3}\t" +
"SunTime2={4}\tSunTime2VSUser={5}\tSunTime2VSDoor={6}\tSunTime3={7}\tSunTime3VSUser={8}\tSunTime3VSDoor={9}\t" +
"MonTime1={10}\tMonTime1VSUser={11}\tMonTime1VSDoor={12}\tMonTime2={13}\tMonTime2VSUser={14}\tMonTime2VSDoor={15}\t" +
"MonTime3={16}\tMonTime3VSUser={17}\tMonTime3VSDoor={18}\tTueTime1={19}\tTueTime1VSUser={20}\tTueTime1VSDoor={21}\t" +
"TueTime2={22}\tTueTime2VSUser={23}\tTueTime2VSDoor={24}\tTueTime3={25}\tTueTime3VSUser={26}\tTueTime3VSDoor={27}\t" +
"WedTime1={28}\tWedTime1VSUser={29}\tWedTime1VSDoor={30}\tWedTime2={31}\tWedTime2VSUser={32}\tWedTime2VSDoor={33}\t" +
"WedTime3={34}\tWedTime3VSUser={35}\tWedTime3VSDoor={36}\tThuTime1={37}\tThuTime1VSUser={38}\tThuTime1VSDoor={39}\t" +
"ThuTime2={40}\tThuTime2VSUser={41}\tThuTime2VSDoor={42}\tThuTime3={43}\tThuTime3VSUser={44}\tThuTime3VSDoor={45}\t" +
"FriTime1={46}\tFriTime1VSUser={47}\tFriTime1VSDoor={48}\tFriTime2={49}\tFriTime2VSUser={50}\tFriTime2VSDoor={51}\t" +
"FriTime3={52}\tFriTime3VSUser={53}\tFriTime3VSDoor={54}\tSatTime1={55}\tSatTime1VSUser={56}\tSatTime1VSDoor={57}\t" +
"SatTime2={58}\tSatTime2VSUser={59}\tSatTime2VSDoor={60}\tSatTime3={61}\tSatTime3VSUser={62}\tSatTime3VSDoor={63}\t" +
"Hol1Time1={64}\tHol1Time1VSUser={65}\tHol1Time1VSDoor={66}\tHol1Time2={67}\tHol1Time2VSUser={68}\tHol1Time2VSDoor={69}\t" +
"Hol1Time3={70}\tHol1Time3VSUser={71}\tHol1Time3VSDoor={72}\tHol2Time1={73}\tHol2Time1VSUser={74}\tHol2Time1VSDoor={75}\t" +
"Hol2Time2={76}\tHol2Time2VSUser={77}\tHol2Time2VSDoor={78}\tHol2Time3={79}\tHol2Time3VSUser={80}\tHol2Time3VSDoor={81}\t" +
"Hol3Time1={82}\tHol3Time1VSUser={83}\tHol3Time1VSDoor={84}\tHol3Time2={85}\tHol3Time2VSUser={86}\tHol3Time2VSDoor={87}\t" +
"Hol3Time3={88}\tHol3Time3VSUser={89}\tHol3Time3VSDoor={90}";
        //门不同的时间段验证方式信息
        public const string Command_UpdateDoorVSTimezone = "DATA UPDATE DoorVSTimezone DoorID={0}\tTimezoneID={1}\tDevID={2}";
        //人不同的时间段验证方式信息
        public const string Command_UpdatePersonalVSTimezone = "DATA UPDATE PersonalVSTimezone PIN={0}\tDoorID={1}\tTimezoneID={2}\tDevID={3}";
        //超级用户权限
        public const string Command_UpdateSuperAuthorize = "DATA UPDATE SuperAuthorize Pin={0}\tDoorID={1}\tDevID={2}";
        //下发一体化模板
        public const string Command_UpdateBioData = "DATA UPDATE BIODATA Pin={0}\tNo={1}\tIndex={2}\tValid={3}\tDuress={4}\tType={5}\tMajorVer={6}\tMinorVer ={7}\tFormat={8}\tTmp={9}";
        //下发比对照片
        public const string Command_UpdateBioPhoto = "DATA UPDATE biophoto PIN={0}\tType={1}\tSize={2}\tContent={3}\tFormat={4}\tUrl={5}\tPostBackTmpFlag={6}";
        //下发用户照片
        public const string Command_UpdateUserPic = "DATA UPDATE userpic pin={0}\tsize={1}\tformat={2}\turl={3}\tcontent={4}";


        //Delete
        //删除用户信息
        public const string Command_DeleteUser = "DATA DELETE user Pin={0}";
        //删除扩展用户
        public const string Command_DeleteExtUser = "DATA DELETE extuser Pin={0}";
        //删除一人多卡数据
        public const string Command_DeleteMulCardUser = "DATA DELETE mulcarduser Pin={0}";
        //删除用户门禁权限
        public const string Command_DeleteUserAuthorize = "DATA DELETE userauthorize Pin={0}";
        //删除门禁记录
        public const string Command_DeleteTransaction = "DATA DELETE transaction {0}";
        //删除指纹模板
        public const string Command_DeleteTemplatev10WithFingerID = "DATA DELETE templatev10 Pin={0}\tFingerID={1}";
        public const string Command_DeleteTemplatev10 = "DATA DELETE templatev10 Pin={0}";
        //删除节假日
        public const string Command_DeleteHoliday = "DATA DELETE holiday {0}";
        //删除时间段
        public const string Command_DeleteTimeZone = "DATA DELETE timezone {0}";
        //删除首卡开门
        public const string Command_DeleteFirstCard = "DATA DELETE firstcard DoorID={0}";
        //删除多卡开门
        public const string Command_DeleteMultimCard = "DATA DELETE multimcard Index={0}";
        //删除联动详细信息
        public const string Command_DeleteInOutFun = "DATA DELETE inoutfun Index={0}";
        //删除定时输出
        public const string Command_DeleteOutRelaySetting = "DATA DELETE outrelaysetting {0}";
        //删除夏令时
        public const string Command_DeleteDSTSetting = "DATA DELETE DSTSetting {0}";
        //删除设备属性
        public const string Command_DeleteDevProperty = "DATA DELETE DevProperty {0}";
        //删除设备参数
        public const string Command_DeleteDevParameters = "DATA DELETE DevParameters {0}";
        //删除门属性
        public const string Command_DeleteDoorProperty = "DATA DELETE DoorProperty {0}";
        //删除门参数
        public const string Command_DeleteDoorParameters = "DATA DELETE  DoorParameters {0}";
        //删除读头属性
        public const string Command_DeleteReaderProperty = "DATA DELETE  ReaderProperty {0}";
        //删除读头参数
        public const string Command_DeleteReaderParameters = "DATA DELETE  ReaderParameters {0}";
        //删除辅助输入
        public const string Command_DeleteAuxIn = "DATA DELETE  AuxIn {0}";
        //删除辅助输出
        public const string Command_DeleteAuxOut = "DATA DELETE  AuxOut {0}";
        //删除默认韦根数据
        public const string Command_DeleteDefaultWGFormat = "DATA DELETE  DefaultWGFormat {0}";
        //删除韦根数据
        public const string Command_DeleteWGFormat = "DATA DELETE  WGFormat {0}";
        //删除读头韦根数据
        public const string Command_DeleteWGFormatReader = "DATA DELETE  WGFormat ReaderID={0}";
        //删除输入控制（受时间段限制）
        public const string Command_DeleteInputIOSetting = "DATA DELETE InputIOSetting {0}";
        //删除不同时段的验证方式数据
        public const string Command_DeleteDiffTimezoneVS = "DATA DELETE  DiffTimezoneVS TimezoneID={0}";
        //删除门不同时段的验证方式数据
        public const string Command_DeleteDoorVSTimezone = "DATA DELETE DoorVSTimezone DoorID={0}";
        //删除人不同时段的验证方式数据
        public const string Command_DeletePersonalVSTimezone = "DATA DELETE  PersonalVSTimezone PIN={0} DoorID={1}";
        //删除超级用户权限数据
        public const string Command_DeleteSuperAuthorize = "DATA DELETE  SuperAuthorize Pin={0}";
        //删除比对照片
        public const string Command_DeleteBioPhoto = "DATA DELETE  biophoto PIN={0}";
        //删除用户照片
        public const string Command_DeleteUserpic = "DATA DELETE  userpic pin={0}";
        //删除一体化模板
        public const string Command_DeleteBioData1 = "DATA DELETE  biodata Type={0}";
        public const string Command_DeleteBioData2 = "DATA DELETE  biodata Type={0} pin={1} no={2}";

        //Count
        //获取用户数量
        public const string Command_CountUser = "DATA COUNT user";
        //获取设备门禁记录数量
        public const string Command_CountTransaction = "DATA COUNT transaction";
        //获取指纹模板数量
        public const string Command_CountTemplate10 = "DATA COUNT template10";
        //获取比对照片数量
        public const string Command_CountBioPhoto1 = "DATA COUNT biophoto";
        public const string Command_CountBioPhoto2 = "DATA COUNT biophoto Type={0}";
        //获取一体化模板数量
        public const string Command_CountBioData1 = "DATA COUNT biodata";
        public const string Command_CountBioData2 = "DATA COUNT biodata Type={0}";


        //Query
        //查询用户
        public const string Command_QueryUser = "DATA QUERY tablename=user,fielddesc=*,filter=*";
        //查询指纹模板
        public const string Command_QueryTemplatev10 = "DATA QUERY tablename=templatev10,fielddesc=*,filter=*";
        //查询门禁记录
        public const string Command_QueryTransaction = "DATA QUERY tablename=transaction,fielddesc=*,filter=*";
        //查询WIFI列表
        public const string Command_QueryAPList = "DATA QUERY tablename=[APList],fielddesc=*,filter=*";
        //查询比对照片
        public const string Command_QueryBioPhoto1 = "DATA QUERY tablename=biophoto,fielddesc=*,filter=*";
        public const string Command_QueryBioPhoto2 = "DATA QUERY tablename=biophoto,fielddesc=*,filter=type={0}";
        //查询一体化模板
        public const string Command_QueryBioData1 = "DATA QUERY tablename=biodata,fielddesc=*,filter=*";
        public const string Command_QueryBioData2 = "DATA QUERY tablename=biodata,fielddesc=*,filter=type={0}";

        //ACOUNT
        //一体机
        public const string Command_AccountAccessControlTerminalTransaction = "ACCOUNT transaction ContentType={0} MaxIndex={1}";
        //控制器
        public const string Command_AccountAccessControlTransaction1 = "ACCOUNT transaction MaxIndex={0}";
        public const string Command_AccountAccessControlTransaction2 = "ACCOUNT transaction StartIndex={0}\tEndIndex={1}";
        public const string Command_AccountAccessControlTransaction3 = "ACCOUNT transaction StartTime={0}\tEndTime={1}";

        //Test Host
        public const string Command_TestHost = "Test Host Address={0}:{1}";

        //Control Device
        public const string Command_ControlDevice = "CONTROL DEVICE {0}";
        public const string Command_ControlCancelAllAlarm = "CONTROL DEVICE 02000000";
        public const string Command_ControlRemoteControlDoor1 = "CONTROL DEVICE 01010105";
        public const string Command_ControlReboot = "CONTROL DEVICE 03000000";


        //SetOptions
        public const string Command_SetOptioins = "SET OPTIONS {0}";
        public const string Command_SetWebServerIPAndPort = "SET OPTIONS WebServerIP={0},WebServerPort={1}";
        public const string Command_SetDoor1Drivertime = "SET OPTIONS Door1Drivertime={0}";

        //GetOptions
        public const string Command_GetOptioins = "Get OPTIONS {0}";
        public const string Command_GetWebServerIPAndPort = "Get OPTIONS WebServerIP,WebServerPort";
        public const string Command_GetDoor1Drivertime = "Get OPTIONS Door1Drivertime";

        //Upgrade
        public const string Command_Upgrade1 = "UPGRADE checksum={0},url={1},size={2}";
        public const string Command_Upgrade2 = "UPGRADE type={0},checksum={1},size={2},url={3}";

        //RemoteVerify
        public const string Command_RemoteVerify = "AUTH={0}\r\n{1}\r\nCONTROL DEVICE {2}";

        //Other
        public const string Command_Unknown = "UNKNOWN";
    }
}
