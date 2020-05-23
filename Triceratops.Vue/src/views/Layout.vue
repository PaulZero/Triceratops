<template>
    <div>
        <site-navigation />
        <router-view />
    </div>    
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import SiteNavigation from '@/components/navigation/SiteNavigation.vue';

    @Component({
        components: {
            SiteNavigation
        }
    })
    export default class Layout extends Vue {

    }
</script>

<style scoped>
</style>