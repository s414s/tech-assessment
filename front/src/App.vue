<script setup lang="ts">
import { RouterView } from 'vue-router';

import Header from './components/Header.vue';
import { toasts } from './helpers/toast';
</script>

<template>
    <v-app>
        <div class="main-container">
            <Header />
            <main class="content">
                <RouterView />
            </main>
            <Footer />
        </div>
        <v-snackbar-queue
            v-model="toasts"
            location="bottom end"
            timeout="6000"
            closable
        />
    </v-app>
</template>

<style scoped>
.main-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;
    min-height: 100vh;
}

.content {
    flex-grow: 1;
    width: 100%;
    padding: 20px
}
</style>
