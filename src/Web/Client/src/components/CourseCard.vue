<script setup lang="ts">
// import { computed } from "vue";
// import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
// import { faStar as faStarReg } from "@fortawesome/free-regular-svg-icons";
// import { faStar as faStarSolid } from '@fortawesome/free-solid-svg-icons'
import TeeTimeList from "./TeeTimeList.vue";

type CourseCardProps = {
    id: string;
    name: string;
    source: string;
    uri: string;
    address: Address;
    photo?: string;
    date: string | null;
    players: string;
    holes: string;
};
defineProps<CourseCardProps>();

// const mapUri = computed(() => {
//     const query = encodeURIComponent(
//         `${props.name} ${props.address.city} ${props.address.state}`
//     );
//     return `http://maps.google.com?q=${query}`;
// });
</script>

<template>
    <article
        class="bg-brand-700 w-full p-4 mb-2 rounded border-2 border-slate-800 relative"
    >
        <div class="flex justify-between">
            <a
                class="text-rose-500 font-bold text-sm"
                target="_blank"
                :href="uri"
                aria-label="Book tee time"
            >
                {{ name }}
            </a>
        </div>
        <p class="text-xs">{{ address.city }}, {{ address.state }}</p>
        <tee-time-list
            :course-id="id"
            :source="source"
            :date="date"
            :players="players"
            :holes="holes"
        />

        <!--    <a target="_blank" :href="mapUri">-->
        <!--      Directions-->
        <!--      <font-awesome-icon :icon="faMap"/>-->
        <!--    </a>-->
    </article>
</template>

<style scoped></style>
