import { ref } from 'vue';

export interface Toast {
    text: string;
    color: string;
}

// Rendered by the <v-snackbar-queue> in App.vue
export const toasts = ref<Toast[]>([]);

export function showError(text: string) {
    toasts.value.push({ text, color: 'error' });
}
