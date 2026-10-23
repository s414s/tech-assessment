import ItemAvailability from '@/views/ItemAvailability.vue';
import ItemsView from '@/views/ItemsView.vue';
import UserBookingsView from '@/views/UserBookingsView.vue';
import { createRouter, createWebHistory } from 'vue-router';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'items',
      component: ItemsView,
    },
    {
      path: '/availability',
      name: 'availability',
      component: ItemAvailability,
    },
    {
      path: '/userBookings',
      name: 'userBookings',
      component: UserBookingsView,
    },
  ],
});

export default router;