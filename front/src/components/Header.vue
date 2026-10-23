<script setup lang="ts">

</script>

<template>
    <v-container>
        <nav class="header-container">
            <RouterLink to="/">Resources</RouterLink>
            <RouterLink to="/myBookings">MyBookings</RouterLink>
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
}

</style>