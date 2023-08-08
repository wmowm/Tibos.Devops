<template>
  <div class="analysis">
    <a-row style="margin-top: 0" :gutter="[24, 24]">
      <a-col :sm="24" :md="12" :xl="6">
        <chart-card :loading="loading" title="构建数量" :total="data.dayBuildCount">
          <a-tooltip title="指标说明" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <trend term="日环比" :target="data.dayBuildCount" :value="data.yesterDayBuildCount" :scale="0" />
          </div>
          <div slot="footer">我的构建数<span> {{ data.myDayBuildCount }}</span></div>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6">
        <chart-card :loading="loading" title="发布数量" :total="data.dayPublishCount">
          <a-tooltip title="指标说明" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-area />
          </div>
          <div slot="footer">我的发布数<span> {{data.myDayPublishCount  }}</span></div>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6">
        <chart-card :loading="loading" title="应用数量" :total="data.appCount">
          <a-tooltip title="指标说明" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-bar :data="data.appRepList"/>
          </div>
          <div slot="footer">我的应用数 <span> {{ data.myAppCount }}</span></div>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6">
        <chart-card :loading="loading" title="用户数量" :total="data.userCount">
          <a-tooltip title="指标说明" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-progress target="60" :percent="(data.activeUserCount/data.userCount)*100" color="#13C2C2" height="8px"/>
          </div>
          <div slot="footer">今日活跃用户数 <span> {{ data.activeUserCount }}</span></div>
        </chart-card>
      </a-col>
    </a-row>
    <a-row style="margin: 0 -12px">
      <a-col style="padding: 0 12px" :xl="12" :lg="24" :md="24" :sm="24" :xs="24">
        <a-card :loading="loading" :bordered="false" style="margin-top: 24px" title="我的应用">
          <hot-search :data="data.myAppList" 
          :cpuData="getCpuNum()" 
          :memoryData="getMemoryNum()" 
          />
        </a-card>
      </a-col>
      <a-col style="padding: 0 12px" :xl="12" :lg="24" :md="24" :sm="24" :xs="24">
        <a-card :loading="loading" :bordered="false" style="margin-top: 24px;" title="资源占比">
          <sales-data :defaultType="this.defaultType" :cpuData="getCpuData()" :memoryData="getMemoryData()" />
          <a-radio-group slot="extra" v-model:value="defaultType" style="margin: -12px 0">
            <a-radio-button value="cpu">CPU</a-radio-button>
            <a-radio-button value="memory">内存</a-radio-button>
          </a-radio-group>
        </a-card>
      </a-col>
    </a-row>
  </div>
</template>

<script>
import { mapGetters} from 'vuex';
import ChartCard from '../../../components/card/ChartCard'
import MiniArea from '../../../components/chart/MiniArea'
import MiniBar from '../../../components/chart/MiniBar'
import MiniProgress from '../../../components/chart/MiniProgress'
import HotSearch from './HotSearch'
import SalesData from './SalesData'
import Trend from '../../../components/chart/Trend'

import {getTopView} from '@/services/workplace';

export default {
  name: 'QueryList',
  components: {Trend, SalesData, HotSearch,  MiniProgress, MiniBar, MiniArea, ChartCard},
  data () {
    return {
      loading: true,
      defaultType:'cpu',
      searchData:{
        envId:0
      },
      data:{}

    }
  },
  created:function()
  {
    this.searchData.envId =this.envId()
    this.getTopView({envId:this.searchData.envId})
  },
  methods: {
    ...mapGetters('account', ['appId','envId']),
    getCpuNum(){
      let data = [];
      if(this.data.nodeMetrics != undefined){
        data = this.data.nodeMetrics.map(item=>{ return {x:item.createTime,y:item.cpu}})
        console.log(data)
      }
      return data
    },
    getMemoryNum(){
      let data = [];
      if(this.data.nodeMetrics != undefined){
        data = this.data.nodeMetrics.map(item=>{ return {x:item.createTime,y:item.memory}})
        console.log(data)
      }
      return data
    },
    getCpuData(){
      let data =[]
      if(this.data.myAppList != undefined){
        data = this.data.myAppList.map(p=>{ return {item:p.name,count:p.cpuUsage}})
        console.log(data)
      }
      return data
    },
    getMemoryData(){
      let data =[]
      if(this.data.myAppList != undefined){
        data = this.data.myAppList.map(p=>{ return {item:p.name,count:p.memoryUsage}})
        console.log(data)
      }
      return data
    },
    getTopView(params){
        this.loading = true
        getTopView(params).then(this.afterGetTopView)    
    },
    afterGetTopView(res){
        this.loading = false
        const data = res.data
        if(data.code == "0"){
            console.log(data.data)
            this.data = data.data
        }else {
            this.$message.error(data.message)
        }
      }
  }
}
</script>
