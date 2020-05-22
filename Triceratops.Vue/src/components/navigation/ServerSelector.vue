<template>
    <li class="nav-item dropdown">
        <a class="nav-link dropdown-toggle" href="#" id="serverSelector" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
            Servers
        </a>
        <div class="dropdown-menu" aria-labelledby="serverSelector">
            <a class="dropdown-item" href="#">Action</a>
            <a class="dropdown-item" href="#">Another action</a>
            <div class="dropdown-divider"></div>
            <router-link to="/servers" class="dropdown-item">All Servers</router-link>
        </div>
    </li>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';

    @Component
    export default class ServerSelector extends Vue {
    }
</script>