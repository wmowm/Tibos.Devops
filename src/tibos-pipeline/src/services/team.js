import {CREATETEAM, GETTEAMBYID,UPDATETEAM,DELETETEAM,GETTEAMLIST,JOINTEAM,GETTEAMUSERLIST,REMOVETEAMUSER,GETNOTJOINUSERLIST,GETGROUPS} from '@/services/api'
import {request, METHOD} from '@/utils/request'


export async function createTeam(params) {
  return request(CREATETEAM, METHOD.POST,params)
}
export async function getTeamById(id) {
  return request(GETTEAMBYID, METHOD.GET,id)
}

export async function updateTeam(params) {
  return request(UPDATETEAM, METHOD.POST,params)
}

export async function deleteTeam(id) {
  return request(DELETETEAM, METHOD.GET,id)
}

export async function getTeamList(params) {
  return request(GETTEAMLIST, METHOD.GET,params)
}

export async function getGroups(params) {
  return request(GETGROUPS, METHOD.GET,params)
}

export async function joinTeam(params) {
  return request(JOINTEAM, METHOD.POST,params)
}

export async function getTeamUserList(params) {
  return request(GETTEAMUSERLIST, METHOD.GET,params)
}

export async function getNotJoinUserList(params) {
  return request(GETNOTJOINUSERLIST, METHOD.GET,params)
}

export async function removeTeamUser(id) {
  return request(REMOVETEAMUSER, METHOD.GET,id)
}