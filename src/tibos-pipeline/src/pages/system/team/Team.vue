<template>
  <a-card>
    <div :class="advanced ? 'search' : null">
      <a-form :form="form" layout="horizontal">
        <div :class="advanced ? null: 'fold'">
        <a-row >
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="团队编号"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
             <a-input v-model="searchData.id" />
            </a-form-item>
          </a-col>
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="团队名称"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
             <a-input v-model="searchData.name" />
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
      <a-space class="operator">
        <a-button @click="createTeam" type="primary">添加团队</a-button>
      </a-space>
    <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="data"
        :pagination="pagination"
        :rowSelection=null
        rowKey="id"
      >

        <div slot="description" slot-scope="{text}">
          {{text}}
        </div>
        <div slot="domains" slot-scope="{text}">
          <div v-if="text.domains !=null && text.domains != ''">
          <a v-for="item in text.domains.split(',')" :key="item">
            {{ item }}
            <br/>
          </a>
        </div>
        </div>

        <div slot="groups" slot-scope="{text}">
          <div v-if="text.groups != ''">
          <a-tag v-for="item in text.groups.split(',')" :key="item" :color="colorList[moduloOperation(item,colorList.length)]">
            {{item}}
          </a-tag>
        </div>
        </div>
        <div slot="action" slot-scope="{text, record}">
          <a @click="joinTeam(record.id,record.name)" style="margin-right: 8px">
            <a-icon type="plus"/>加入团队
          </a>
          <a @click="getTeamUser(record.id)" style="margin-right: 8px">
            <a-icon type="search"/>查看团队用户
          </a>
          <a @click="updateTeam(record.id)" style="margin-right: 8px">
            <a-icon type="edit"/>编辑
          </a>
          <a-popconfirm
          v-if="data.length"
          title="确定删除?"
          @confirm="() => deleteTeam(record.id)"
        >
          <a href="javascript:;"><a-icon type="delete" />删除</a>
        </a-popconfirm>
          <!-- <router-link :to="`/list/query/detail/${record.key}`" >详情</router-link> -->
        </div>
        <template slot="statusTitle">
          <a-icon @click.native="onStatusTitleClick" type="info-circle" />
        </template>
      </standard-table>
    </a-spin>
    </div>
     <add-or-edit-team ref="addOrEditTeam" @refresh= "refresh" />

     <add-team-user ref="addTeamUser" @refresh= "refresh" />
     <team-user ref="teamUser" @refresh= "refresh" />

  </a-card>
</template>

<script>
import StandardTable from '@/components/table/StandardTable'
import {getTeamList,deleteTeam} from '@/services/team';
import {moduloOperation} from '@/services/common';
import AddOrEditTeam from './AddOrEditTeam.vue';
import AddTeamUser from './AddTeamUser.vue';
import TeamUser from './TeamUser.vue';
const columns = [
  {
    title: '团队编号',
    dataIndex: 'id',
    key: 'id'
  },
  {
    title: '团队名称',
    dataIndex: 'name',
    key: 'name',
  },
  {
    title: '授权组',
    key: 'groups',
    scopedSlots: { customRender: 'groups' }
  },
  {
    title: '域名',
    key: 'domains',
    scopedSlots: { customRender: 'domains' }
  },
  {
    title: '团队描述',
    dataIndex: 'remark',
    key: 'remark'
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
  components: {StandardTable,AddOrEditTeam,AddTeamUser,TeamUser},
  data () {
    return {
      advanced: false,
      columns: columns,
      data: [],
      searchData:{
        pageIndex:1,
        pageSize:10,
        id: '',
        name: ''
      },
      form: this.$form.createForm(this, { name: 'search' }),
      pagination: {},
      selectedRows: [],
      spinning:false,
      colorList:["pink","red","orange","green","cyan","blue","purple"]
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
     this.getTeamList(this.searchData)   
  },
  methods: {
    handleSubmit(e){
      e.preventDefault();
      this.getTeamList(this.searchData)
    },
    reset() {
      this.searchData = {
        id: '',
        name: '',
        remark: '',
        pageIndex:1,
        pageSize:10
      }
    },
    moduloOperation(txt,leng){
      return moduloOperation(txt,leng)
    },
    refresh(){
      this.searchData.pageIndex = 1
      this.pagination.current = 1
      this.getTeamList(this.searchData)
    },
    getTeamList(params){
        this.spinning = true
        getTeamList(params).then(this.afterGetTeamList)    
    },
    afterGetTeamList(res){
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
    deleteTeam(id) {
      this.spinning = true
      deleteTeam({id:id}).then(this.afterDeleteTeam)    
     
    },
    afterDeleteTeam(res){
      this.spinning = false
      const data = res.data;
      if (data.code == '0') {
            this.$message.success('操作成功!');
            this.refresh();
          } else {
            this.$message.error(data.message)
        }
    },
    toggleAdvanced () {
      this.advanced = !this.advanced
    },
    changePage(pageIndex,pageSize) {
         this.pagination.current = pageIndex
         this.searchData.pageIndex = pageIndex
         this.searchData.pageSize = pageSize
        this.getTeamList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getTeamList(this.searchData)  
    },
    createTeam() {
      this.$refs.addOrEditTeam.actionType = 0
      this.$refs.addOrEditTeam.initData('')
      this.$refs.addOrEditTeam.showDrawer()
    },
    updateTeam (id) {
      this.$refs.addOrEditTeam.actionType = 1
      this.$refs.addOrEditTeam.initData(id)
      this.$refs.addOrEditTeam.showDrawer()
    },
    joinTeam(id,name){
      this.$refs.addTeamUser.initData(id,name)
      this.$refs.addTeamUser.showDrawer()
    },
    getTeamUser(id){
      this.$refs.teamUser.initData(id)
      this.$refs.teamUser.showDrawer()
    },
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
