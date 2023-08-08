import {GETPUBLISHLIST,CREATEPUBLISH,ROLLBACKPUBLISH} from '@/services/api'
import {request, METHOD} from '@/utils/request'


export async function getPublishList(params) {
  return request(GETPUBLISHLIST, METHOD.GET,params)
}


export async function createPublish(params) {
  return request(CREATEPUBLISH, METHOD.POST,params)
}

export async function rollBackPublish(params) {
  return request(ROLLBACKPUBLISH, METHOD.POST,params)
}