import Vue from 'vue'
import Vuex from 'vuex'
import ApiStore from './ts/stores/api/ApiStore';

Vue.use(Vuex)

export default new Vuex.Store({
    modules: {
        ApiStore
    }
})
