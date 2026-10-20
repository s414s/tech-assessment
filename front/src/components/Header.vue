<script setup lang="ts">
import { useUserStore } from '@/stores/userStore';
import { storeToRefs } from 'pinia';

const userStore = useUserStore();
const { logout } = useUserStore();
const { isAuthed, isAdmin } = storeToRefs(userStore);

</script>

<template>
    <v-container v-if="isAuthed">
        <nav class="header-container">
            <RouterLink to="/">Home</RouterLink>
            <RouterLink to="/hotels">Hotels</RouterLink>
            <RouterLink to="/bookings">Bookings</RouterLink>
            <RouterLink v-if="isAdmin" to="/dashboard">Dashboard</RouterLink>
            <RouterLink to="/user">User</RouterLink>
            <a href="#" @click="logout">Log out</a>
        </nav>
    </v-container>

    <v-container v-if="!isAuthed">
        <nav class="header-container-landing" v-if="!isAuthed">
            <RouterLink to="/login">Login</RouterLink>
        </nav>
    </v-container>
</template>

<style scoped>
.header-container {
    display: flex;
    flex-direction: row;
    justify-content: center;
    padding: 2rem 0rem;
    gap: 2rem;
    /* border: 2px solid green; */
    /* width: 100vw; */
}

.header-container-landing {
    display: flex;
    flex-direction: row;
    justify-content: end;
    padding: 2rem 0rem;
    gap: 2rem;
    /* border: 2px solid green; */
    /* width: 100vw; */
}

a,
a:visited {
    color: black;
    text-decoration: none;
}
</style>