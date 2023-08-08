import {GETPODLIST,RESTARTPOD,SCALEPOD,GETPODLOG} from '@/services/api'
import {request, METHOD} from '@/utils/request'


export async function getPodList(params) {
  return request(GETPODLIST, METHOD.GET,params)
}


export async function restartPod(params) {
  return request(RESTARTPOD, METHOD.POST,params)
}

export async function scalePod(params) {
  return request(SCALEPOD, METHOD.POST,params)
}

export async function getPodLog(params) {
  return request(GETPODLOG, METHOD.GET,params)
}