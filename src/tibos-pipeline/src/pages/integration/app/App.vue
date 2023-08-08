<template>
  <a-card>
    <div :class="advanced ? 'search' : null">
      <a-form :form="form" layout="horizontal">
        <div :class="advanced ? null: 'fold'">
        <a-row >
        <a-col :md="8" :sm="24" >
            <a-form-item 
              label="项目"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
            <a-select :allowClear=allowClear 
                      v-model:value="searchData.projectId"               
                      placeholder="请选择项目"
            >
            <a-select-option v-for="item in this.projectList" :key="item.id">
            {{item.name}}
            </a-select-option>
          </a-select>
            </a-form-item>
          </a-col>


          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="应用编号"
              :labelCol="{span: 5}"
              :wrapperCol="{span: 18, offset: 1}"
            >
             <a-input v-model="searchData.id" />
            </a-form-item>
          </a-col>
          <a-col :md="8" :sm="24" >
            <a-form-item 
              label="应用名称"
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
        <a-button :disabled="disabled" @click="createApp" type="primary">添加应用</a-button>
      </a-space>
    <a-spin :spinning="spinning">
      <standard-table
        :columns="columns"
        :dataSource="data"
        :pagination="pagination"
        :rowSelection=null
        rowKey="id"
      >

        <div slot="webUrl" slot-scope="{text}">
          <a :href="text.webUrl" target="_blank">{{ text.webUrl }}</a>
        </div>
        <div slot="name" slot-scope="{text}">
          <my-icon v-if="!favoriteAppList.some(item => item.id === text.id)" style="font-size:14px" type="icon-shoucang1" @click="favoriteApp(text.id,true)" title="加入常用" />
          <my-icon v-else style="font-size:14px" type="icon-shoucang" @click="favoriteApp(text.id,false)" title="取消常用"/>
          <a>
            {{ text.name }}
          </a>
        </div>


        <div slot="remark" slot-scope="{text, record}">
          {{ record.remark }}
          <a @click="updateApp(record.id,record.remark)" style="margin-left: 8px">
                    <a-icon type="edit"/>
            </a>
        </div>

        <div slot="action" slot-scope="{text, record}">
          <a @click="showApp(record.id)" style="margin-right: 8px">
            <a-icon type="search"/>查看详情
          </a>
          <a-popconfirm
          v-if="data.length"
          title="确定删除?"
          @confirm="() => deleteApp(record.id)"
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
     <add-app ref="addApp" @refresh= "refresh" />
     <edit-app ref="editApp" @refresh= "refresh" />
     <show-app ref="showApp" @refresh= "refresh" />
  </a-card>
</template>

<script>
import {mapMutations} from 'vuex'
import StandardTable from '@/components/table/StandardTable'
import {getAppList,deleteApp,favoriteApp} from '@/services/app';
import {getUserProjectList,getFavoriteAppList} from '@/services/user';
import AddApp from './AddApp.vue';
import EditApp from './EditApp.vue';
import ShowApp from './ShowApp.vue';
import {Icon} from 'ant-design-vue';
const MyIcon = Icon.createFromIconfontCN({scriptUrl:"//at.alicdn.com/t/c/font_4178161_bjirbk4s4z9.js"})

const columns = [
  {
    title: '应用编号',
    dataIndex: 'id',
    key: 'id'
  },
  {
    title: '应用名称',
    key: 'name',
    scopedSlots: { customRender: 'name' },
  },
  {
    title: '组',
    dataIndex: 'group',
    key: 'group',
  },
  {
    title: 'gitlab地址',
    key: 'webUrl',
    scopedSlots: { customRender: 'webUrl' },
  },
  {
    title: '项目名称',
    dataIndex: 'projectName',
    key: 'projectName'
  },
  {
    title: '应用描述',
    dataIndex: 'remark',
    key: 'remark',
    scopedSlots: { customRender: 'remark' },
  },
  {
    title: '创建人',
    dataIndex: 'createUserName',
    key: 'createUserName'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime'
  },
  {
    title: '操作',
    scopedSlots: { customRender: 'action' },
    width: '10%'
  }
]
export default {
  name: 'QueryList',
  components: {StandardTable,AddApp,MyIcon,EditApp,ShowApp},
  data () {
    return {
      disabled:true,
      allowClear:true,
      advanced: false,
      columns: columns,
      data: [],
      projectList:[],
      favoriteAppList:[],
      searchData:{
        pageIndex:1,
        pageSize:10,
        projectId:'',
        id: '',
        name: ''
      },
      form: this.$form.createForm(this, { name: 'search' }),
      pagination: {},
      selectedRows: [],
      spinning:false
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
     this.getUserProjectList()
     this.getAppList(this.searchData)   
     this.getFavoriteAppList()

     const searchParams = new URLSearchParams(window.location.search);
     const projectId = searchParams.get("projectId");
      
      if(projectId != '' && projectId != null){
        this.searchData.projectId = parseInt(projectId)
      }
  },
  watch:{
    'searchData.projectId'(newVal){
      if(newVal == undefined || newVal == ''){
          this.disabled = true
      }else{
        this.disabled = false
      }
    },
    // 监听路由变化
    "$route.path": function() {
      const searchParams = new URLSearchParams(window.location.search);
      const projectId = searchParams.get("projectId");
        
        if(projectId != '' && projectId != null){
          this.searchData.projectId = parseInt(projectId)
        }
    }
  },
  methods: {
    ...mapMutations('account', ['setMyAppList']),
    getFavoriteAppList(){
      getFavoriteAppList().then(this.afterGetFavoriteAppList)  
    },
    afterGetFavoriteAppList(res){
      const data = res.data
      if(data.code == '0'){
        this.favoriteAppList = data.data
        this.setMyAppList(data.data)

      }else{
            this.$message.error(data.message)
        }
    },
    favoriteApp(appId,favoriteType){
      favoriteApp({appId:appId,favoriteType:favoriteType}).then(this.afterFavoriteApp)  
    },
    afterFavoriteApp(res){
      const data = res.data
      if(data.code == '0'){
        this.$message.success('操作成功!');
        this.getFavoriteAppList()
        this.refresh()
      }
    },

    handleSubmit(e){
      e.preventDefault();
      this.getAppList(this.searchData)
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
    refresh(){
      this.searchData.pageIndex = 1
      this.pagination.current = 1
      this.getAppList(this.searchData)
    },
    getAppList(params){
        this.spinning = true
        getAppList(params).then(this.afterGetAppList)    
    },
    afterGetAppList(res){
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
    getUserProjectList(params){
        this.spinning = true
        getUserProjectList(params).then(this.afterGetUserProjectList)    
    },
    afterGetUserProjectList(res){
        this.spinning = false
        const data = res.data
        if(data.code == "0"){
            this.projectList = data.data
        }else {
            this.$message.error(data.message)
        }
    },
    deleteApp(id) {
      this.spinning = true
      deleteApp({id:id}).then(this.afterDeleteApp)    
     
    },
    afterDeleteApp(res){
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
        this.getAppList(this.searchData)  
    },
    sizeChange(pageIndex,pageSize){
        this.searchData.pageIndex = 1
        this.searchData.pageSize = pageSize
        this.pagination.current = 1
        this.getAppList(this.searchData)  
    },
    createApp() {
      const project = this.projectList.find(v=>v.id == this.searchData.projectId)
      this.$refs.addApp.initData(project.id,project.name)
      this.$refs.addApp.showDrawer()
    },
    updateApp (id,remark) {
      this.$refs.editApp.initData(id,remark)
      this.$refs.editApp.showDrawer()
    },
    showApp (id) {
      this.$refs.showApp.initData(id)
      this.$refs.showApp.showDrawer()
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
