<script setup lang="ts">
import { defineProps } from 'vue';

const props = defineProps({
    isLoading: Boolean,
});

</script>

<template>
    <div v-if="props.isLoading">
        <v-container>
            <v-row justify="center">
                <v-progress-circular color="primary" indeterminate :size="43"></v-progress-circular>
            </v-row>
        </v-container>
    </div>
</template>