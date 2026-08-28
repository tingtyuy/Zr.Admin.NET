import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

/* Layout */
import Layout from '@/layout'

/**
 * Note: 路由配置项
 *
 * hidden: true                   // 当设置 true 的时候该路由不会再侧边栏出现 如401，login等页面，或者如一些编辑页面/edit/1
 * alwaysShow: true               // 当你一个路由下面的 children 声明的路由大于1个时，自动会变成嵌套的模式--如组件页面
 *                                // 只有一个时，会将那个子路由当做根路由显示在侧边栏--如引导页面
 *                                // 若你想不管路由下面的 children 声明的个数都显示你的根路由
 *                                // 你可以设置 alwaysShow: true，这样它就会忽略之前定义的规则，一直显示根路由
 * redirect: noRedirect           // 当设置 noRedirect 的时候该路由在面包屑导航中不可被点击
 * name:'router-name'             // 设定路由的名字，一定要填写不然使用<keep-alive>时会出现各种问题
 * meta : {
    noCache: true                // 如果设置为true，则不会被 <keep-alive> 缓存(默认 false)
    title: 'title'               // 设置该路由在侧边栏和面包屑中展示的名字
    icon: 'svg-name'             // 设置该路由的图标，对应路径src/assets/icons/svg
    breadcrumb: false            // 如果设置为false，则不会在breadcrumb面包屑中显示
  }
 */

// 公共路由
export const constantRoutes = [{
    path: '/redirect',
    component: Layout,
    hidden: true,
    children: [{
      path: '/redirect/:path(.*)',
      component: (resolve) => require(['@/views/redirect'], resolve)
    }]
  },
  {
    path: '/login',
    component: (resolve) => require(['@/views/login'], resolve),
    hidden: true
  },
	{
    path: '/register',
    component: (resolve) => require(['@/views/register'], resolve),
    hidden: true
  },
  {
    path: '/404',
    component: (resolve) => require(['@/views/error/404'], resolve),
    hidden: true
  },
  {
    path: '/401',
    component: (resolve) => require(['@/views/error/401'], resolve),
    hidden: true
  },
  {
    path: '',
    component: Layout,
    redirect: 'index',
    children: [{
      path: 'index',
      component: (resolve) => require(['@/views/index'], resolve),
      name: 'Index',
      meta: { title: '首页', icon: 'dashboard', affix: true }
    }],
  },
  {
    path: '/user',
    component: Layout,
    hidden: true,
    redirect: 'noredirect',
    children: [{
      path: 'profile',
      component: (resolve) => require(['@/views/system/user/profile/index'], resolve),
      name: 'Profile',
      meta: { title: '个人中心', icon: 'user' }
    }]
  },
	{
    path: '/echarts',
    component: (resolve) => require(['@/views/components/Echarts'], resolve),
    hidden: true
  },{
    path: '/icons',
    component: (resolve) => require(['@/views/components/icons/index'], resolve),
    hidden: true
  },
  {
    path: '/comfyui',
    component: Layout,
    redirect: '/comfyui/workflow',
    name: 'ComfyuiModule',
    meta: { title: 'ComfyUI管理', icon: 'tool' },
    children: [
      {
        path: 'workflow',
        component: (resolve) => require(['@/views/comfyui/workflow'], resolve),
        name: 'ComfyuiWorkflow',
        meta: { title: '工作流管理', icon: 'upload' }
      },
      {
        path: 'txt2img',
        component: (resolve) => require(['@/views/comfyui/txt2img'], resolve),
        name: 'ComfyuiTxt2Img',
        meta: { title: '文生图', icon: 'edit' }
      },
      {
        path: 'img2img',
        component: (resolve) => require(['@/views/comfyui/img2img'], resolve),
        name: 'ComfyuiImg2Img',
        meta: { title: '图生图', icon: 'picture' }
      },
      {
        path: 'txt2video',
        component: (resolve) => require(['@/views/comfyui/txt2video'], resolve),
        name: 'ComfyuiTxt2Video',
        meta: { title: '文生视频', icon: 'video' }
      },
      {
        path: 'img2video',
        component: (resolve) => require(['@/views/comfyui/img2video'], resolve),
        name: 'ComfyuiImg2Video',
        meta: { title: '图生视频', icon: 'video-camera' }
      },
      {
        path: 'task-list',
        component: (resolve) => require(['@/views/comfyui/taskList'], resolve),
        name: 'ComfyuiTaskList',
        meta: { title: '任务列表', icon: 'documentation' }
      },
      {
        path: 'task-queue',
        component: (resolve) => require(['@/views/comfyui/taskQueue'], resolve),
        name: 'ComfyuiTaskQueue',
        meta: { title: '执行队列', icon: 'list' }
      },
      {
        path: 'settings',
        component: (resolve) => require(['@/views/comfyui/settings'], resolve),
        name: 'ComfyuiSettings',
        meta: { title: '服务设置', icon: 'setting' }
      }
    ]
  },  {
    path: '/ai',
    component: Layout,
    redirect: '/ai/submit',
    name: 'AiModule',
    meta: { title: 'AI功能', icon: 'guide' },
    children: [
      {
        path: 'submit',
        component: (resolve) => require(['@/views/ai/submit'], resolve),
        name: 'AiSubmit',
        meta: { title: 'AI图生图', icon: 'peoples' }
      },
      {
        path: 'list',
        component: (resolve) => require(['@/views/ai/list'], resolve),
        name: 'AiList',
        meta: { title: '任务列表', icon: 'documentation' }
      },
      {
        path: 'result/:taskNo',
        component: (resolve) => require(['@/views/ai/result'], resolve),
        name: 'AiResult',
        meta: { title: '任务详情', icon: 'documentation', hidden: true }
      },
      {
        path: 'edit/:taskNo',
        component: (resolve) => require(['@/views/ai/edit'], resolve),
        name: 'AiEdit',
        meta: { title: '编辑任务', icon: 'documentation', hidden: true }
      }
    ]
  },
]

export default new Router({
  base: process.env.VUE_APP_ROUTER_PREFIX,
  mode: 'history', // 去掉url中的#
  // scrollBehavior: () => ({ y: 0 }),
  routes: constantRoutes
})