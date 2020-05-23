import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'
import Servers from './views/Servers.vue';

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
        path: '/servers',
        name: 'server-list',
        component: Servers
    }
  ]
})
