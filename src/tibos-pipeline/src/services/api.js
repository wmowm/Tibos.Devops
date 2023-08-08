
const BASE_URL = process.env.VUE_APP_API_BASE_URL
module.exports = {
  GETAUTHORIZE : `${BASE_URL}/api/oauth/getauthorize`,
  GITLABLOGIN : `${BASE_URL}/api/oauth/gitlablogin`,
  ROUTES: `${BASE_URL}/api/system/routes`,


  //#region 项目管理
  CREATEPROJECT: `${BASE_URL}/api/projectinfo/create`,
  GETPROJECTBYID: `${BASE_URL}/api/projectinfo/getbyid`,
  UPDATEPROJECT: `${BASE_URL}/api/projectinfo/update`,
  DELETEPROJECT: `${BASE_URL}/api/projectinfo/delete`,
  GETPROJECTLIST: `${BASE_URL}/api/projectinfo/getlist`,
  //#endregion

  //#region 应用管理
  CREATEAPP: `${BASE_URL}/api/appinfo/create`,
  GETAPPBYID: `${BASE_URL}/api/appinfo/getbyid`,
  UPDATEAPP: `${BASE_URL}/api/appinfo/update`,
  DELETEAPP: `${BASE_URL}/api/appinfo/delete`,
  GETAPPLIST: `${BASE_URL}/api/appinfo/getlist`,
  GETBUILDLIST: `${BASE_URL}/api/appinfo/getbuildlist`,
  FAVORITEAPP: `${BASE_URL}/api/appinfo/favoriteapp`,
  GETSUCCESSBUILDLIST: `${BASE_URL}/api/appinfo/getsuccessbuildlist`,
  //#endregion

  //#region 团队管理
    CREATETEAM: `${BASE_URL}/api/teaminfo/create`,
    GETTEAMBYID: `${BASE_URL}/api/teaminfo/getbyid`,
    UPDATETEAM: `${BASE_URL}/api/teaminfo/update`,
    DELETETEAM: `${BASE_URL}/api/teaminfo/delete`,
    GETTEAMLIST: `${BASE_URL}/api/teaminfo/getlist`,
    GETGROUPS: `${BASE_URL}/api/teaminfo/getgroups`,


    JOINTEAM: `${BASE_URL}/api/teaminfo/jointeam`,
    GETTEAMUSERLIST: `${BASE_URL}/api/teaminfo/getteamuserlist`,
    GETNOTJOINUSERLIST: `${BASE_URL}/api/teaminfo/getnotjoinuserlist`,
    REMOVETEAMUSER: `${BASE_URL}/api/teaminfo/removeteamuser`,
    //#endregion

  //#region 用户管理
  CREATEUSERLOGIN: `${BASE_URL}/api/userinfo/createuserlogin`,
  GETUSERLOGIN: `${BASE_URL}/api/userinfo/getuserlogin`,
  UPDATEUSERINFO: `${BASE_URL}/api/userinfo/updateuserinfo`,
  UPDATEUSERSTATUS: `${BASE_URL}/api/userinfo/updateuserstatus`,
  GETUSERINFOLIST: `${BASE_URL}/api/userinfo/getuserinfolist`,
  GETROLES: `${BASE_URL}/api/userinfo/getroles`,
  //#endregion

  //#region 我的信息
  LOGIN: `${BASE_URL}/api/myinfo/login`,
  UPDATEUSERPWD: `${BASE_URL}/api/myinfo/updateuserpwd`,
  GETUSERTEAMLIST: `${BASE_URL}/api/myinfo/getuserteamlist`,
  GETUSERPROJECTLIST: `${BASE_URL}/api/myinfo/getuserprojectlist`,
  GETFAVORITEAPPLIST: `${BASE_URL}/api/myinfo/getfavoriteapplist`,
  //#endregion


  //#region 模板管理
  CREATETEMPLATE: `${BASE_URL}/api/templateinfo/create`,
  GETTEMPLATEBYID: `${BASE_URL}/api/templateinfo/getbyid`,
  UPDATETEMPLATE: `${BASE_URL}/api/templateinfo/update`,
  DELETETEMPLATE: `${BASE_URL}/api/templateinfo/delete`,
  GETTEMPLATELIST: `${BASE_URL}/api/templateinfo/getlist`,
  DOWNLOAD: `${BASE_URL}/api/templateinfo/download`,
  //#endregion

    //#region 环境管理
    CREATEENV: `${BASE_URL}/api/envinfo/create`,
    GETENVBYID: `${BASE_URL}/api/envinfo/getbyid`,
    UPDATEENV: `${BASE_URL}/api/envinfo/update`,
    DELETEENV: `${BASE_URL}/api/envinfo/delete`,
    GETENVLIST: `${BASE_URL}/api/envinfo/getlist`,
    UPDATEMAPPINGCONFIG: `${BASE_URL}/api/envinfo/updatemappingconfig`,
    UPDATECHECKPUBLISH: `${BASE_URL}/api/envinfo/updatecheckpublish`,
    //#endregion


    //#region 部署管理
    GETPUBLISHLIST: `${BASE_URL}/api/deployment/getpublishlist`,
    CREATEPUBLISH: `${BASE_URL}/api/deployment/createpublish`,
    ROLLBACKPUBLISH:`${BASE_URL}/api/deployment/RollBackPublish`,
    //#endregion

    //#region 容器管理
    GETPODLIST: `${BASE_URL}/api/container/GetPodList`,
    RESTARTPOD: `${BASE_URL}/api/container/RestartPod`,
    SCALEPOD:`${BASE_URL}/api/container/ScalePod`,
    GETPODLOG:`${BASE_URL}/api/container/GetPodLog`,
    //#endregion

    //#region 配置字典
    GETLIST: `${BASE_URL}/api/configmap/GetList`,
    GETBYID: `${BASE_URL}/api/configmap/GetById`,
    DELETE:`${BASE_URL}/api/configmap/Delete`,
    CREATE:`${BASE_URL}/api/configmap/Create`,
    UPDATE:`${BASE_URL}/api/configmap/Update`,
    UPDATECONFIGCONTENT:`${BASE_URL}/api/configmap/UpdateConfigContent`,
    UPDATECONFIGSTATUS:`${BASE_URL}/api/configmap/UpdateConfigStatus`,
    REDEPLOYMENT:`${BASE_URL}/api/configmap/Redeployment`,
    GETCONFIGRECORD:`${BASE_URL}/api/configmap/GetConfigRecord`,
    //#endregion

    //#region 工作台
    GETTOPVIEW: `${BASE_URL}/api/WorkPlace/getTopView`,
    //#endregion
  }
