import {CREATETEMPLATE, GETTEMPLATEBYID,UPDATETEMPLATE,DELETETEMPLATE,GETTEMPLATELIST,DOWNLOAD} from '@/services/api'
import {request, METHOD,exportFile} from '@/utils/request'


export async function createTemplate(params) {
  return request(CREATETEMPLATE, METHOD.POST,params)
}
export async function getTemplateById(id) {
  return request(GETTEMPLATEBYID, METHOD.GET,id)
}

export async function updateTemplate(params) {
  return request(UPDATETEMPLATE, METHOD.POST,params)
}

export async function deleteTemplate(id) {
  return request(DELETETEMPLATE, METHOD.GET,id)
}

export async function getTemplateList(params) {
  return request(GETTEMPLATELIST, METHOD.GET,params)
}

export function download(params) {
  return exportFile(DOWNLOAD, METHOD.GET,params,'zip')
}
