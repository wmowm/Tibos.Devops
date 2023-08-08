<template>
  <a-card>
    <div :class="advanced ? 'search' : null">
      <a-form :form="form" layout="horizontal">
        <div :class="advanced ? null: 'fold'">
        <a-row >
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="手机号"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
             <a-input v-model="searchData.phone" />
            </a-form-item>
          </a-col>
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="名称"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
             <a-input v-model="searchData.nickName" />
            </a-form-item>
          </a-col>
        </a-row>
        </div>
        <span style="float: right; margin-top: 3px;">
          <a-button @click="handleSubmit" type="primary">查询</a-button>
          <a-button @click="reset" style="margin-left: 8px">重置</a-button>
          <a @click="toggleAdvanced" style="margin-left: 8px">
            {{advanced ? '收起' : '展开'}}
            <a-icon :type="advanced ? 'up' : 'down'" />
          </a>
        </span>
      </a-form>
    </div>
    <div>
    <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="data"
        :pagination="pagination"
        :rowSelection=null
        rowKey="id"
      >
        <!-- <div slot="teamId" slot-scope="{text}">
          <div v-if="text.teamId !=null">
            {{ $refs.addOrEditUser.teamList.find(v => v.id===text.teamId).name }}
          </div>
        </div> -->
        <span slot="nickName" slot-scope="{text}">
         <a-avatar size="large" :src= text.avatarUrl />
         {{text.nickName}}
        </span>

        <div slot="loginType" slot-scope="{text}">
          <img v-for="item in text.loginTypes" :key="item" :src="item===0?require(`@/assets/img/account.png`):require(`@/assets/img/gitlab.png`)" />
        </div>

        <div slot="group" slot-scope="{text}">
          <div v-if="text.group != ''">
          <a-tag v-for="item in text.group.split(',')" :key="item" :color="colorList[moduloOperation(item,colorList.length)]">
            {{item}}
          </a-tag>
        </div>
        </div>
        <div slot="roles" slot-scope="{text}">
          <div v-if="text.roles != ''">

          <a-tag v-for="item in text.roles.split(',')" :key="item" :color="roleColorMap[item]">
            {{item}}
          </a-tag>
        </div>
        </div>

        <div slot="status" slot-scope="{text}">
          <a-switch :checked="text.status === 1 ? true : false" @change="userStatusChange($event,text.id)" checkedChildren="启用" unCheckedChildren="禁用" />
        </div>

        <div slot="action" slot-scope="{text, record}">

          <a v-if="!record.loginTypes.includes(0)" @click="createUserLogin(record.id)" style="margin-right: 8px">
            <a-icon type="plus"/>添加账号
          </a>

          <a v-if="record.loginTypes.includes(0) && !record.roles.includes('Admin')" @click="editUserInfo(record.id)" style="margin-right: 8px">
            <a-icon type="edit"/>编辑
          </a>
          <!-- <router-link :to="`/list/query/detail/${record.key}`" >详情</router-link> -->
        </div>
        <template slot="statusTitle">
          <a-icon @click.native="onStatusTitleClick" type="info-circle" />
        </template>
      </standard-table>
    </a-spin>
    </div>
     <add-user-account ref="addUserAccount" @refresh= "refresh" />
     <edit-user-info ref="editUserInfo" @refresh= "refresh" />
  </a-card>
</template>

<script>
import StandardTable from '@/components/table/StandardTable'
import {getUserInfoList,updateUserStatus} from '@/services/user';
import {moduloOperation} from '@/services/common';
import AddUserAccount from './AddUserAccount.vue';
import EditUserInfo from './EditUserInfo.vue';
const columns = [
  {
    title: '用户',
    key: 'nickName',
    scopedSlots: { customRender: 'nickName' }
  },
  {
    title: '手机号',
    dataIndex: 'phone',
    key: 'phone'
  },
  {
    title: '角色',
    key: 'roles',
    scopedSlots: { customRender: 'roles' },
  },
  {
    title: '状态',
    key: 'status',
    scopedSlots: { customRender: 'status' },
  },
  {
    title: '登录方式',
    key: 'loginType',
    scopedSlots: { customRender: 'loginType' },
  },
  {
    title: '授权组',
    key: 'group',
    scopedSlots: { customRender: 'group' },
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime'
  },
  {
    title: '操作',
    scopedSlots: { customRender: 'action' },
    width: '25%'
  }
]
export default {
  name: 'QueryList',
  components: {StandardTable,AddUserAccount,EditUserInfo},
  data () {
    return {
      advanced: false,
      columns: columns,
      data: [],
      searchData:{
        pageIndex:1,
        pageSize:10,
        phone: '',
        nickName: ''
      },
      form: this.$form.createForm(this, { name: 'search' }),
      pagination: {},
      selectedRows: [],
      spinning:false,
      colorList:["pink","red","orange","green","cyan","blue","purple"],
      roleColorMap:{Admin:"#cd201f",Developer:"#108ee9",Manager:"#3b5999",Test:"#f50",Other:"#87d068"},
    }
  },
//   authorize: {
//     deleteRecord: 'delete'
//   },
  created:function()
  {
    this.pagination = {        
        pageIndex:this.searchData.pageIndex,
        pageSize:this.searchData.pageSize,
        current:1,
        showSizeChanger: true, // 显示可改变每页数量
        pageSizeOptions: ['10', '20', '50', '100'], // 每页数量选项
        showTotal: total => `Total ${total} items`, // 显示总数
        onShowSizeChange: (current, pageSize) => this.sizeChange(current,pageSize), // 改变每页数量时更新显示
        onChange:(pageIndex,pageSize)=>this.changePage(pageIndex,pageSize),//点击页码事件
        total:0 //总条数
        },
     this.getUserInfoList(this.searchData)   
  },
  methods: {
    userStatusChange(checked, id){
      console.log(`a-switch to ${checked}`, id)
      this.spinning = true
      updateUserStatus({userId:id,status:checked?1:0}).then(this.afterUpdateUserStatus)   

    },
    handleSubmit(e){
      e.preventDefault();
      this.getUserInfoList(this.searchData)
    },
    moduloOperation(txt,leng){
      return moduloOperation(txt,leng)
    },
    reset() {
      this.searchData = {
        phone: '',
        nickName: '',
        pageIndex:1,
        pageSize:10
      }
    },
    refresh(){
      this.searchData.pageIndex = 1
      this.pagination.current = 1
      this.getUserInfoList(this.searchData)
    },
    getUserInfoList(params){
        this.spinning = true
        getUserInfoList(params).then(this.afterGetUserInfoList)    
    },
    afterGetUserInfoList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            console.log(data.data)
            this.data = data.data
            this.pagination.total=data.total;
        }else {
            this.$message.error(data.message)
        }
    },
    afterUpdateUserStatus(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.getUserInfoList(this.searchData)
        }else{
            this.$message.error(data.message)
        }
    },
    // deleteUser(id) {
    //   this.spinning = true
    //   deleteUser({id:id}).then(this.afterDeleteUser)    
     
    // },
    // afterDeleteUser(res){
    //   this.spinning = false
    //   const data = res.data;
    //   if (data.code == '0') {
    //         this.$message.success('操作成功!');
    //         this.refresh();
    //       } else {
    //         this.$message.error(data.message)
    //     }
    // },
    toggleAdvanced () {
      this.advanced = !this.advanced
    },
    changePage(pageIndex,pageSize) {
         this.pagination.current = pageIndex
         this.searchData.pageIndex = pageIndex
         this.searchData.pageSize = pageSize
        this.getUserInfoList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getUserInfoList(this.searchData)  
    },
    createUserLogin(userId) {
      this.$refs.addUserAccount.initData(userId)
      this.$refs.addUserAccount.showDrawer()
    },
    editUserInfo (userId) {
      this.$refs.editUserInfo.initData(userId)
      this.$refs.editUserInfo.showDrawer()
    }
  }
}
</script>

<style lang="less" scoped>
  .search{
    margin-bottom: 54px;
  }
  .fold{
    width: calc(100% - 216px);
    display: inline-block
  }
  .operator{
    margin-bottom: 18px;
  }
  @media screen and (max-width: 900px) {
    .fold {
      width: 100%;
    }
  }
</style>
