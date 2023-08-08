// 视图组件
const view = {
  tabs: () => import('@/layouts/tabs'),
  blank: () => import('@/layouts/BlankView'),
  page: () => import('@/layouts/PageView')
}

// 路由组件注册
const routerMap = {
  login: {
    authority: '*',
    path: '/login',
    component: () => import('@/pages/login')
  },
  root: {
    path: '/',
    name: '首页',
    redirect: '/login',
    component: view.tabs
  },
  dashboard: {
    name: '仪盘表',
    component: view.blank
  },
  integration: {
    name: '持续集成',
    component: view.blank
  },
  deployment: {
    name: '部署管理',
    component: view.blank
  },
  system: {
    name: '系统管理',
    component: view.blank
  },
  workplace: {
    name: '工作台',
    component: () => import('@/pages/dashboard/workplace')
  },
  project: {
    name: '项目列表',
    component: () => import('@/pages/integration/project')
  },
  app: {
    name: '应用列表',
    component: () => import('@/pages/integration/app')
  },
  build: {
    name: '构建记录',
    component: () => import('@/pages/integration/build')
  },
  user: {
    name: '用户列表',
    component: () => import('@/pages/system/user')
  },
  team: {
    name: '团队列表',
    component: () => import('@/pages/system/team')
  },
  template: {
    name: '模板列表',
    component: () => import('@/pages/system/template')
  },
  env: {
    name: '环境设置',
    component: () => import('@/pages/system/env')
  },
  publish: {
    name: '快速部署',
    component: () => import('@/pages/deployment/publish')
  },
  container: {
    name: '容器管理',
    component: () => import('@/pages/deployment/container')
  },
  configmap: {
    name: '配置字典',
    component: () => import('@/pages/deployment/configmap')
  },
  configrecord: {
    name: '配置记录',
    component: () => import('@/pages/deployment/configrecord')
  },

  // success: {
  //   name: '成功',
  //   component: () => import('@/pages/result/Success')
  // },
  // error: {
  //   name: '失败',
  //   component: () => import('@/pages/result/Error')
  // },
  exception: {
    name: '异常页',
    icon: 'warning',
    component: view.blank
  },
  exp403: {
    authority: '*',
    name: 'exp403',
    path: '403',
    component: () => import('@/pages/exception/403')
  },
  exp404: {
    name: 'exp404',
    path: '404',
    component: () => import('@/pages/exception/404')
  },
  exp500: {
    name: 'exp500',
    path: '500',
    component: () => import('@/pages/exception/500')
  },
}
export default routerMap

