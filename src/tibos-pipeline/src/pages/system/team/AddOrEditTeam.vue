<template>
  <div>
    <a-drawer
      :title="actionType == 0 ? `新增团队`: `编辑团队` "
      :width="480"
      :visible="visible"
      :body-style="{ paddingBottom: '80px' }"
      @close="onClose"
    >

     <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol">

        <a-form-model-item ref="name" label="团队名称" prop="name">
          <a-input
            v-model="form.name"
          />
        </a-form-model-item>
        
        <a-form-model-item label="组" prop='groups'>
            <a-select  
                      v-model="form.groups"                 
                      mode="multiple" 
                      placeholder="请选择组"
                      @change="handleChange"
            >
            <a-select-option v-for="item in this.groupList" :key="item.name">
            {{item.name}}
            </a-select-option>
          </a-select>
        </a-form-model-item>     
        <a-form-model-item
          v-for="(domain, index) in form.domains"
          :key="domain.key"
          :label="'域名' + (index+1)"
          :rules="{ required: true, message: '请输入域名', trigger: 'blur' }"
        >
          <a-input
            v-model:value="domain.value"
            addon-before="http://"
            placeholder="请输入域名"
            style="width: 90%;"
          >
          </a-input>
          <a-icon type="delete"
            v-if="form.domains.length > 1"
            style="margin-left: 10px"
            :disabled="form.domains.length === 1"
            @click="removeDomain(domain)"
          />
        </a-form-model-item>
        <a-form-model-item label="">
          <a-button type="dashed" @click="addDomain" style="float: right;">
            <a-icon type="plus" />
            添加
          </a-button>
          
        </a-form-model-item>

        <a-form-model-item ref="remark" label="描述" prop="remark">
          <a-textarea
          placeholder="" :default-value="form.remark" v-model="form.remark"
          :auto-size="{ minRows: 2, maxRows: 6 }"
        />
        </a-form-model-item>
      </a-form-model>
       <div
        :style="{
          position: 'absolute',
          right: 0,
          bottom: 0,
          width: '100%',
          borderTop: '1px solid #e9e9e9',
          padding: '10px 16px',
          background: '#fff',
          textAlign: 'right',
          zIndex: 1,
        }"
      >
        <a-button :style="{ marginRight: '8px' }" @click="onClose">
          取消
        </a-button>
        <a-button type="primary" @click="onSubmit">
          提交
        </a-button>
      </div>


    </a-drawer>
  </div>
</template>

<script>
import {getTeamById,createTeam,updateTeam,getGroups} from '@/services/team';
export default {
  name: 'AddOrEditTeam',
  components: {},
  data () {
    return {
      actionType: 0,
      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 6 },
      wrapperCol: { span: 18 },
      fileList: [],
      uploading: false,
      groupList:[],
      form: {
        id: '',
        name: '',
        remark:'',
        groups:[],
        domains:[],
        domains0:''
      },
     
      rules:{
        name: [
          { required: true, message: '请输入团队名称', trigger: 'blur' },
        ],
      },
    }
  },
  created:function()
  {

  },
  methods: {
    initData(id){
      this.form.id = id
      if(id=='' || id==null || id== undefined){
      this.form.id=''
      this.form.name = ''
      this.form.remark = ''
      this.form.groups = []
      }else{
        getTeamById({id:id}).then(this.afterGetTeamById)  
      }
      this.getGroups()
    },
    removeDomain(item) {
      let index = this.form.domains.indexOf(item);
      if (index !== -1) {
        this.form.domains.splice(index, 1);
      }
    },
    addDomain(){
      this.form.domains.push({
        value: '',
        key: Date.now() + (Math.random()*(9999-0)+0),
      });
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    getGroups(){
      getGroups().then(this.afterGetGroups)  
    },
    afterGetGroups(res){
      const data = res.data
      if(data.code == '0'){
        this.groupList = data.data
      }else{
            this.$message.error(data.message)
        }
    },
    afterGetTeamById(res){
      const data = res.data
      if(data.code == '0'){
        this.form = data.data
        if(data.data.groups !=null && data.data.groups != ''){
          this.form.groups = data.data.groups.split(',');
        }else{
          this.form.groups =[]
        }
        if(data.data.domains !=null && data.data.domains != ''){
          const domains = data.data.domains.split(',');
          this.form.domains = domains.map(obj=>{return {
                      value: obj,
                      key: Date.now() + (Math.random()*(9999-0)+0),
                    }})
        }else{
          this.form.domains =[]
        }
      }else{
            this.$message.error(data.message)
        }
    },
    handleChange(value){
      console.log(value)
    },
    onSubmit () {
      this.$refs.ruleForm.validate(async valid => {
        if (valid) {
          let res = {}
          this.form.domains = this.form.domains.map(obj=>{return obj.value})
          if(this.actionType == 0){
              res = await createTeam(this.form)
          }else{
              res = await updateTeam(this.form)
          }
          const data = res.data;
          if (data.code == '0') {
            this.uploading = false
            this.$message.success('操作成功!');
            this.$emit('refresh');
            this.onClose()

          } else {
            this.uploading = false
            this.$message.error(data.message)
          }
        } else {
          return false
        }
      })
    },
    toggleAdvanced () {
      this.advanced = !this.advanced
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
