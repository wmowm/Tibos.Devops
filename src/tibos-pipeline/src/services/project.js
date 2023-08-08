import {CREATEPROJECT, GETPROJECTBYID,UPDATEPROJECT,DELETEPROJECT,GETPROJECTLIST} from '@/services/api'
import {request, METHOD} from '@/utils/request'


export async function createProject(params) {
  return request(CREATEPROJECT, METHOD.POST,params)
}
export async function getProjectById(id) {
  return request(GETPROJECTBYID, METHOD.GET,id)
}

export async function updateProject(params) {
  return request(UPDATEPROJECT, METHOD.POST,params)
}

export async function deleteProject(id) {
  return request(DELETEPROJECT, METHOD.GET,id)
}

export async function getProjectList(params) {
  return request(GETPROJECTLIST, METHOD.GET,params)
}
