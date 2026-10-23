import ItemBookingsView from '@/views/ItemBookingsView.vue';
import UserBookingsView from '@/views/UserBookingsView.vue';
import { createRouter, createWebHistory } from 'vue-router';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'bookings',
      component: ItemBookingsView,
    },
    {
      path: '/mybookings',
      name: 'mybookings',
      component: UserBookingsView,
    },
  ],
});

export default router;