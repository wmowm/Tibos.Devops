import {CREATEENV, GETENVBYID,UPDATEENV,DELETEENV,GETENVLIST,UPDATEMAPPINGCONFIG,UPDATECHECKPUBLISH} from '@/services/api'
import {request, METHOD} from '@/utils/request'


export async function createEnv(params) {
  return request(CREATEENV, METHOD.POST,params)
}
export async function getEnvById(id) {
  return request(GETENVBYID, METHOD.GET,id)
}

export async function updateEnv(params) {
  return request(UPDATEENV, METHOD.POST,params)
}

export async function deleteEnv(id) {
  return request(DELETEENV, METHOD.GET,id)
}

export async function getEnvList(params) {
  return request(GETENVLIST, METHOD.GET,params)
}
export async function updateMappingConfig(params) {
  return request(UPDATEMAPPINGCONFIG, METHOD.POST,params)
}

export async function updateCheckPublish(params) {
  return request(UPDATECHECKPUBLISH, METHOD.POST,params)
}