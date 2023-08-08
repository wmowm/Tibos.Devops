import {GETLIST,GETBYID,DELETE,CREATE,UPDATE,UPDATECONFIGCONTENT,UPDATECONFIGSTATUS,REDEPLOYMENT,GETCONFIGRECORD} from '@/services/api'
import {request, METHOD} from '@/utils/request'


export async function getConfigMapList(params) {
  return request(GETLIST, METHOD.GET,params)
}


export async function getById(params) {
  return request(GETBYID, METHOD.GET,params)
}

export async function deleteConfigMap(params) {
  return request(DELETE, METHOD.GET,params)
}

export async function createConfigMap(params) {
  return request(CREATE, METHOD.POST,params)
}
export async function updateConfigMap(params) {
  return request(UPDATE, METHOD.POST,params)
}
export async function updateConfigContent(params) {
  return request(UPDATECONFIGCONTENT, METHOD.POST,params)
}
export async function updateConfigStatus(params) {
  return request(UPDATECONFIGSTATUS, METHOD.POST,params)
}
export async function redeployment(params) {
  return request(REDEPLOYMENT, METHOD.POST,params)
}

export async function getConfigRecord(params) {
  return request(GETCONFIGRECORD, METHOD.GET,params)
}