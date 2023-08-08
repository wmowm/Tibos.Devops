import {CREATEAPP, GETAPPBYID,UPDATEAPP,DELETEAPP,GETAPPLIST,GETBUILDLIST,FAVORITEAPP,GETSUCCESSBUILDLIST} from '@/services/api'
import {request, METHOD} from '@/utils/request'


export async function createApp(params) {
  return request(CREATEAPP, METHOD.POST,params)
}
export async function getAppById(id) {
  return request(GETAPPBYID, METHOD.GET,id)
}

export async function updateApp(params) {
  return request(UPDATEAPP, METHOD.POST,params)
}

export async function deleteApp(id) {
  return request(DELETEAPP, METHOD.GET,id)
}

export async function getAppList(params) {
  return request(GETAPPLIST, METHOD.GET,params)
}
export async function getBuildList(params) {
  return request(GETBUILDLIST, METHOD.GET,params)
}

export async function favoriteApp(params) {
  return request(FAVORITEAPP, METHOD.POST,params)
}

export async function getSuccessBuildList(params) {
  return request(GETSUCCESSBUILDLIST, METHOD.GET,params)
}