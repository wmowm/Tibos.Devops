import {LOGIN, ROUTES,GETAUTHORIZE,GITLABLOGIN,CREATEUSERLOGIN,GETUSERLOGIN,UPDATEUSERINFO,UPDATEUSERSTATUS,GETUSERINFOLIST,GETROLES,UPDATEUSERPWD,GETUSERTEAMLIST,GETUSERPROJECTLIST,GETFAVORITEAPPLIST} from '@/services/api'
import {request, METHOD, removeAuthorization} from '@/utils/request'

/**
 * 登录服务
 * @param userName 账户名
 * @param password 账户密码
 * @returns {Promise<AxiosResponse<T>>}
 */
export async function login(params) {
  console.log(LOGIN)
  return request(LOGIN, METHOD.POST, params)
}

export async function getAuthorize() {
  return request(GETAUTHORIZE, METHOD.GET)
}
export async function gitlabLogin(code) {
  return request(GITLABLOGIN, METHOD.GET,code)
}

export async function getRoutesConfig() {
  return request(ROUTES, METHOD.GET)
}

export async function createUserLogin(params) {
  return request(CREATEUSERLOGIN, METHOD.POST,params)
}
export async function getUserLogin(params) {
  return request(GETUSERLOGIN, METHOD.GET,params)
}
export async function updateUserInfo(params) {
  return request(UPDATEUSERINFO, METHOD.POST,params)
}
export async function updateUserStatus(params) {
  return request(UPDATEUSERSTATUS, METHOD.POST,params)
}
export async function getUserInfoList(params) {
  return request(GETUSERINFOLIST, METHOD.GET,params)
}
export async function getRoles() {
  return request(GETROLES, METHOD.GET)
}

export async function updateUserPwd(params) {
  return request(UPDATEUSERPWD, METHOD.POST,params)
}
export async function getUserTeamList() {
  return request(GETUSERTEAMLIST, METHOD.GET)
}
export async function getUserProjectList() {
  return request(GETUSERPROJECTLIST, METHOD.GET)
}
export async function getFavoriteAppList() {
  return request(GETFAVORITEAPPLIST, METHOD.GET)
}
/**
 * 退出登录
 */
export function logout() {
  localStorage.removeItem(process.env.VUE_APP_ROUTES_KEY)
  localStorage.removeItem(process.env.VUE_APP_PERMISSIONS_KEY)
  localStorage.removeItem(process.env.VUE_APP_ROLES_KEY)
  removeAuthorization()
}
export default {
  login,
  logout,
  getRoutesConfig
}
