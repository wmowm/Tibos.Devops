<template>
    <a-drawer
      title="查看用户"
      :width="1280"
      :visible="visible"
      :body-style="{ paddingBottom: '80px' }"
      @close="onClose"
    >
  <a-card>
    <div :class="advanced ? 'search' : null">
      <a-form :form="form" layout="horizontal">
        <div :class="advanced ? null: 'fold'">
        <a-row >
          <a-col :md="8" :sm="24" >
            <a-form-item
              label="用户名称"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
            <a-input v-model="searchData.userName" />
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

      <span slot="teamName" slot-scope="{text}">
         <a-avatar size="large" :src= text.logo />
         {{text.teamName}}
        </span>
        <span slot="userName" slot-scope="{text}">
         <a-avatar size="large" :src= text.avatarUrl />
         {{text.userName}}
        </span>

        <div slot="action" slot-scope="{text, record}">
        <a-popconfirm
          v-if="data.length"
          title="确定删除?"
          @confirm="() => deleteRecord(record.teamId,record.userId)"
        >
          <a href="javascript:;"><a-icon type="delete" />删除</a>
        </a-popconfirm>
        </div>

      </standard-table>
    </a-spin>
    </div>
  </a-card>
    </a-drawer>
</template>

<script>
import StandardTable from '@/components/table/StandardTable'
import {getTeamUserList,removeTeamUser,joinTeam} from '@/services/team';
const columns = [
  {
    title: '团队',
    key: 'teamName',
    scopedSlots: { customRender: 'teamName' }
  },
  {
    title: '用户',
    key: 'userName',
    scopedSlots: { customRender: 'userName' }
  },
  {
    title: '加入时间',
    dataIndex: "createTime",
    key: 'createTime',
  },
  {
    title: '操作',
    scopedSlots: { customRender: 'action' },
    width: '10%'
  }
]
export default {
  name: 'TeamUser',
  components: {StandardTable},
  data () {
    return {
      advanced: false,
      spinning:false,
      visible:false,
      columns: columns,
      data: [],
      teamId: 0,
      form: this.$form.createForm(this, { name: 'search' }),
      searchData: {
        userName: '',
        teamId: '',
        pageIndex: 1,
        pageSize: 10
        },
        pagination: {

        },
      selectedRows: [],
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
        onChange:(page,pageSize)=>this.changePage(page,pageSize),//点击页码事件
        total:0 //总条数
        }
  },
  methods: {
    initData(id){
      console.log(id)
      this.searchData.teamId = id
      this.getTeamUserList(this.searchData)   
    },
    handleSubmit(e){
      e.preventDefault();
      this.getTeamUserList(this.searchData)
    },
    reset() {
      this.searchData.userName = ''
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
      this.$emit('refresh');
    },
    getTeamUserList(params){
        this.spinning = true
        getTeamUserList(params).then(this.afterGetTeamUserList)    
    },
    afterGetTeamUserList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.data = data.data
            this.pagination.total=data.total;
        }else {
            this.$message.error(data.message)
        }
    },
    deleteRecord(teamId,userId) {
     this.spinning = true
     removeTeamUser({teamId:teamId,userIds:userId}).then(this.afterRemoveTeamUser)    
    },
    afterRemoveTeamUser(res){
      this.spinning = false
      const data = res.data;
      if (data.code == '0') {
            this.$message.success('操作成功!');
            this.getTeamUserList(this.searchData)
          } else {
            this.$message.error(data.message)
        }
    },
    joinTeam(params){
        this.spinning = true
        joinTeam(params).then(this.afterJoinTeam)     
    },
    afterJoinTeam(res){
      this.spinning = false
      const data = res.data;
      if (data.code == '0') {
            this.$message.success('操作成功!');
            this.GetTeamUserList(this.searchData)
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
        this.GetTeamUserList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.GetTeamUserList(this.searchData) 
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
