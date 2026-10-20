<script setup lang="ts">
import type { VDataTable } from 'vuetify/components';
import type { Booking, ReadonlyHeaders } from '@/types/responseTypes';
import { useRouter } from 'vue-router';

const router = useRouter();
const props = defineProps<{ bookings: Booking[]; }>();
const headers: ReadonlyHeaders = [
    { title: 'Storey', align: 'center', key: 'storey' },
    { title: 'Type', align: 'center', key: 'type' },
    { title: 'Capacity', align: 'center', key: 'capacity' },
];
</script>

<template>
    <v-data-table :headers="headers" :items="props.bookings" density="compact" :sort-by="[{ key: 'id', order: 'asc' }]">
        <template v-slot:item="{ item }">
            <tr @click="router.push({ path: `/newbooking/${item.id}` })" style="cursor: pointer">
                <td align="center"> {{ item.from }}</td>
                <td align="center"> {{ item.to }}</td>
                <td align="center">{{ item.itemType }}</td>
                <td align="center">{{ item.location }}</td>
            </tr>
        </template>
    </v-data-table>
</template>