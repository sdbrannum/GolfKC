<script setup lang="ts">
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import { faUser, faFlag } from "@fortawesome/free-regular-svg-icons";
type TeeTime = {
    rate: number;
    players: number;
    holes: number;
    time: string;
};
defineProps<TeeTime>();
const formatTimeForView = (time: string) => {
    const parts = time.split(":");
    let hours = parseInt(parts[0]);
    const meridian = hours >= 12 ? "PM" : "AM";
    hours = hours > 12 ? hours - 12 : hours;
    return `${hours}:${parts[1]} ${meridian}`;
};
</script>

<template>
    <li
        class="rounded-md border border-slate-300 shadow text-nowrap overflow-hidden"
        style="min-width: 120px"
    >
        <div class="bg-slate-600 text-violet-50 px-1">
            <time class="text-lg font-semibold">
                {{ formatTimeForView(time) }}
            </time>
        </div>
        <div class="px-2 pt-1.5 pb-2">
            <div>
                <font-awesome-icon :icon="faFlag" />
                <span class="pl-1">{{ holes }}</span>
            </div>
            <div class="flex justify-between">
                <span>
                    <font-awesome-icon :icon="faUser" />
                    <span class="pl-1">{{ players }}</span>
                </span>
                <span class="font-bold"><sup>$</sup>{{ rate }}</span>
            </div>
        </div>
    </li>
</template>

<style scoped></style>
