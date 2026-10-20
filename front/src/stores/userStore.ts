import { ref, reactive, computed } from 'vue';
import { defineStore } from 'pinia';
import type { UserInfo } from '@/types/responseTypes';
import { jwtDecode } from "jwt-decode";

interface MyJwtPayload {
    aud: string;
    exp: number;
    iat: number;
    id: string;
    iss: string;
    nbf: number;
    userrole: string;
}

export const useUserStore = defineStore('user', () => {
    console.log("inicializando userStore");
    console.log("valor del localstorage", localStorage.getItem("token"));
    decodeToken(localStorage.getItem("token") ?? "");

    const isAdmin = ref<boolean>(decodeToken(localStorage.getItem("token"))?.userrole === "Admin" ? true : false);
    const isAuthed = ref<boolean>(localStorage.getItem("token") ? true : false);
    const jwt = ref<string | null>(localStorage.getItem("token") ? localStorage.getItem("token") : null);
    const user = reactive<UserInfo>({ id: 0, name: "name", surname: "surname", role: "role" });

    function setJwt(newJwt: string) {
        window.localStorage.setItem("token", newJwt);
        jwt.value = newJwt;
    }

    function decodeToken(token: string | null): MyJwtPayload | null {
        return token ? jwtDecode<MyJwtPayload>(token) : null;
    }

    function setIsAuthed(value: boolean) {
        isAuthed.value = value;
    }

    function fetchUser() {
        const baseUrl = "http://localhost:5093/";
        fetch(baseUrl + 'auth/me', {
            headers: {
                "Authorization": `Bearer ${localStorage.getItem("token") ?? ""}`
            }
        })
            .then(res => res.json())
            .then(data => {
                user.id = data.id;
                user.name = data.name;
                user.surname = data.surname;
                user.role = data.role;

                isAuthed.value = true;
                isAdmin.value = user.role === "Admin";
            });
    }

    function logout() {
        user.id = 0;
        user.name = "";
        user.surname = "";
        user.role = "";

        isAuthed.value = false;
        isAdmin.value = false;

        jwt.value = null;
        localStorage.removeItem("token");
    }

    return { user, isAuthed, isAdmin, fetchUser, jwt, setJwt, setIsAuthed, logout };
});