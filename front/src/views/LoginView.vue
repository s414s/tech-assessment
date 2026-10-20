<script setup lang="ts">
import { useUserStore } from '@/stores/userStore';
import { ref } from 'vue';
import { useRouter } from 'vue-router';
const { fetchUser, setJwt, setIsAuthed, isAuthed } = useUserStore();

const router = useRouter();

const username = ref('');
const password = ref('');

function login() {
    const baseUrl = "http://localhost:5093/";

    console.log("calling login");
    console.log('Username:', username.value);
    console.log('Password:', password.value);

    const requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            username: username.value,
            password: password.value,
        })
    };

    fetch(baseUrl + "auth/login", requestOptions)
        .then(res => {
            if (!res.ok) throw new Error(res.statusText);
            return res.json();
        })
        .then(data => {
            console.log(data);
            setJwt(data.token);
            setIsAuthed(true);
            fetchUser();
            router.push({ path: '/' });
        })
        .catch(e => console.error(e));
}

</script>

<template>
    <main>
        <v-sheet class="mx-auto" width="300">
            <h3>Log in</h3>
            <v-form @submit.prevent>
                <v-text-field v-model="username" label="Username" type="text">
                </v-text-field>
                <v-text-field v-model="password" hide-details="auto" label="Password" type="password">
                </v-text-field>

                <v-btn class="mt-2" @click="login" color="info" type="submit" block>Submit</v-btn>
            </v-form>
        </v-sheet>
    </main>
</template>