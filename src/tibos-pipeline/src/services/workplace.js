import {GETTOPVIEW} from '@/services/api'
import {request, METHOD} from '@/utils/request'


export async function getTopView(params) {
  return request(GETTOPVIEW, METHOD.GET,params)
}