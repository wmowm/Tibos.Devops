export default {
  namespaced: true,
  state: {
    user: undefined,
    permissions: null,
    roles: null,
    routesConfig: null,
    myAppList:null,
    myEnvList:null,
    appId:null,
    envId:null
  },
  getters: {
    user: state => {
      if (!state.user) {
        try {
          const user = localStorage.getItem(process.env.VUE_APP_USER_KEY)
          state.user = JSON.parse(user)
        } catch (e) {
          console.error(e)
        }
      }
      return state.user
    },
    permissions: state => {
      if (!state.permissions) {
        try {
          const permissions = localStorage.getItem(process.env.VUE_APP_PERMISSIONS_KEY)
          state.permissions = JSON.parse(permissions)
          state.permissions = state.permissions ? state.permissions : []
        } catch (e) {
          console.error(e.message)
        }
      }
      return state.permissions
    },
    roles: state => {
      if (!state.roles) {
        try {
          const roles = localStorage.getItem(process.env.VUE_APP_ROLES_KEY)
          state.roles = JSON.parse(roles)
          state.roles = state.roles ? state.roles : []
        } catch (e) {
          console.error(e.message)
        }
      }
      return state.roles
    },
    routesConfig: state => {
      if (!state.routesConfig) {
        try {
          const routesConfig = localStorage.getItem(process.env.VUE_APP_ROUTES_KEY)
          state.routesConfig = JSON.parse(routesConfig)
          state.routesConfig = state.routesConfig ? state.routesConfig : []
        } catch (e) {
          console.error(e.message)
        }
      }
      return state.routesConfig
    },
    myAppList: state => {
      if (!state.myAppList) {
        try {
          const myAppList = localStorage.getItem(process.env.VUE_APP_MYAPPLIST_KEY)
          state.myAppList = JSON.parse(myAppList)
        } catch (e) {
          console.error(e)
        }
      }
      return state.myAppList
    },
    myEnvList: state => {
      if (!state.myEnvList) {
        try {
          const myEnvList = localStorage.getItem(process.env.VUE_APP_MYENVLIST_KEY)
          state.myEnvList = JSON.parse(myEnvList)
        } catch (e) {
          console.error(e)
        }
      }
      return state.myEnvList
    },
    appId: state => {
      if (!state.appId) {
        try {
          const appId = localStorage.getItem(process.env.VUE_APP_APP_KEY)
          state.appId = JSON.parse(appId)
        } catch (e) {
          console.error(e)
        }
      }
      return state.appId
    },
    envId: state => {
      if (!state.envId) {
        try {
          const envId = localStorage.getItem(process.env.VUE_APP_ENV_KEY)
          state.envId = JSON.parse(envId)
        } catch (e) {
          console.error(e)
        }
      }
      return state.envId
    },
  },
  mutations: {
    setUser (state, user) {
      state.user = user
      localStorage.setItem(process.env.VUE_APP_USER_KEY, JSON.stringify(user))
    },
    setPermissions(state, permissions) {
      state.permissions = permissions
      localStorage.setItem(process.env.VUE_APP_PERMISSIONS_KEY, JSON.stringify(permissions))
    },
    setRoles(state, roles) {
      state.roles = roles
      localStorage.setItem(process.env.VUE_APP_ROLES_KEY, JSON.stringify(roles))
    },
    setRoutesConfig(state, routesConfig) {
      state.routesConfig = routesConfig
      localStorage.setItem(process.env.VUE_APP_ROUTES_KEY, JSON.stringify(routesConfig))
    },
    setMyAppList (state, myAppList) {
      state.myAppList = myAppList
      localStorage.setItem(process.env.VUE_APP_MYAPPLIST_KEY, JSON.stringify(myAppList))
    },
    setMyEnvList (state, myEnvList) {
      state.myEnvList = myEnvList
      localStorage.setItem(process.env.VUE_APP_MYENVLIST_KEY, JSON.stringify(myEnvList))
    },

    setAppId(state, appId) {
      state.appId = appId
      localStorage.setItem(process.env.VUE_APP_APP_KEY, JSON.stringify(appId))
    },
    setEnvId(state, envId) {
      state.envId = envId
      localStorage.setItem(process.env.VUE_APP_ENV_KEY, JSON.stringify(envId))
    },
  }
}
