<template>
    <nav id="site-navigation" class="navbar navbar-expand-sm navbar-dark bg-dark">
        <router-link to="/" class="navbar-brand">Triceratops</router-link>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse" id="navbarSupportedContent">
            <ul class="navbar-nav mr-auto">
                <server-selector />
            </ul>
        </div>
    </nav>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import ServerSelector from '@/components/navigation/ServerSelector.vue';

    @Component({
        components: {
            ServerSelector
        }
    })
    export default class SiteNavigation extends Vue {
    }
</script>

<style scoped>
</style>