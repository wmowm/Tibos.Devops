import Mock from 'mockjs'

Mock.mock(`${process.env.VUE_APP_API_BASE_URL}/routes`, 'get', () => {
  let result = {}
  result.code = 0
  result.data = [{
    router: 'root',
    children: [
      {
        router: 'dashboard',
        children: ['workplace','test',{
          router: 'analysis',
          //authority: 'analysis'
        }],
      },
      {
        router: 'form',
        children: ['basicForm', 'stepForm', 'advanceForm','primaryList']
      },
      {
        router: 'basicForm',
        name: '验权表单111',
        icon: 'file-excel',
        authority: 'queryForm'
      },
      {
        router: 'antdv',
        path: 'antdv',
        name: 'Ant Design Vue',
        icon: 'ant-design'
      },
      {
        router: 'document',
        path: 'document',
        name: '使用文档',
        icon: 'file-word'
      }
    ]
  }]
  return result
})
